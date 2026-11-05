export const environment = {
  production: false,
  // Matches the `http` launch profile — no dev-certificate trust step needed.
  apiUrl: 'http://localhost:5045/api',
  supabase: {
    url: 'https://tknydjjvgmhmpzwslhua.supabase.co',
    // Project Settings → API Keys → anon / publishable key.
    // Safe to ship in the browser bundle; that is what it is for.
    anonKey: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRrbnlkamp2Z21obXB6d3NsaHVhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODYwODQxOTIsImV4cCI6MjEwMTY2MDE5Mn0.ywZxmtw1oT1yOCZVZ4aZHrpBOWYsaYQrYjCU2Ts_1Ak',
  },
};
