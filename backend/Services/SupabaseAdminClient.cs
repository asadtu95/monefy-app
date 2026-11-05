using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

/// <summary>
/// Talks to Supabase's admin auth endpoints with the service_role key so accounts
/// can be created already confirmed, skipping the confirmation email.
/// </summary>
/// <remarks>
/// The service_role key bypasses every Supabase security rule, so it lives in
/// user-secrets / environment variables and never leaves the server. This class
/// is deliberately the only thing that holds it, and it exposes exactly one
/// operation rather than a general-purpose admin surface.
/// </remarks>
public class SupabaseAdminClient(HttpClient http, IConfiguration configuration) : ISupabaseAdminClient
{
    private string? ServiceRoleKey => configuration["Supabase:ServiceRoleKey"];

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ServiceRoleKey);

    public async Task<Guid> CreateConfirmedUserAsync(
        string email, string password, string? fullName, CancellationToken ct = default)
    {
        if (!IsConfigured)
        {
            throw new ValidationFailedException(
                "Sign-up is unavailable: Supabase:ServiceRoleKey is not configured on the API. " +
                "Set it with: dotnet user-secrets set \"Supabase:ServiceRoleKey\" \"<service_role key>\".");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "admin/users")
        {
            Content = JsonContent.Create(new CreateUserRequest
            {
                Email = email,
                Password = password,
                EmailConfirm = true,
                UserMetadata = fullName is null ? null : new Dictionary<string, string>
                {
                    ["full_name"] = fullName
                }
            })
        };

        request.Headers.Add("apikey", ServiceRoleKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ServiceRoleKey);

        var response = await http.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ValidationFailedException(DescribeFailure(response.StatusCode, body));
        }

        var created = System.Text.Json.JsonSerializer.Deserialize<CreatedUser>(body);

        return created?.Id is { } id && id != Guid.Empty
            ? id
            : throw new ValidationFailedException("Supabase did not return a user id.");
    }

    /// <summary>Turns Supabase's error payloads into something a form can display.</summary>
    private static string DescribeFailure(HttpStatusCode status, string body)
    {
        try
        {
            var error = System.Text.Json.JsonSerializer.Deserialize<SupabaseError>(body);
            var message = error?.Message ?? error?.ErrorDescription ?? error?.Error;

            if (!string.IsNullOrWhiteSpace(message))
            {
                // The duplicate-account wording differs across Supabase versions.
                if (message.Contains("already", StringComparison.OrdinalIgnoreCase)
                    && message.Contains("regist", StringComparison.OrdinalIgnoreCase))
                {
                    return "An account with that email already exists.";
                }

                return message;
            }
        }
        catch (System.Text.Json.JsonException)
        {
            // Fall through to the generic message below.
        }

        return status == HttpStatusCode.Unauthorized || status == HttpStatusCode.Forbidden
            ? "Supabase rejected the service_role key. Check Supabase:ServiceRoleKey."
            : $"Supabase could not create the account (HTTP {(int)status}).";
    }

    private sealed class CreateUserRequest
    {
        [JsonPropertyName("email")] public string Email { get; init; } = string.Empty;
        [JsonPropertyName("password")] public string Password { get; init; } = string.Empty;

        /// <summary>The whole point: marks the address verified at creation time.</summary>
        [JsonPropertyName("email_confirm")] public bool EmailConfirm { get; init; }

        [JsonPropertyName("user_metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? UserMetadata { get; init; }
    }

    private sealed class CreatedUser
    {
        [JsonPropertyName("id")] public Guid Id { get; init; }
    }

    private sealed class SupabaseError
    {
        [JsonPropertyName("message")] public string? Message { get; init; }
        [JsonPropertyName("error")] public string? Error { get; init; }
        [JsonPropertyName("error_description")] public string? ErrorDescription { get; init; }
        [JsonPropertyName("msg")] public string? Msg { get; init; }
    }
}
