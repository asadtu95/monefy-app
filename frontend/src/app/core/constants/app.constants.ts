export const STORAGE_KEYS = {
  accessToken: 'monefy.access_token',
  refreshToken: 'monefy.refresh_token',
  user: 'monefy.user',
  currency: 'monefy.currency',
} as const;

export const SUPPORTED_CURRENCIES = [
  { code: 'PKR', symbol: '₨', label: 'Pakistani Rupee' },
  { code: 'USD', symbol: '$', label: 'US Dollar' },
  { code: 'EUR', symbol: '€', label: 'Euro' },
  { code: 'GBP', symbol: '£', label: 'British Pound' },
  { code: 'INR', symbol: '₹', label: 'Indian Rupee' },
] as const;

/** Palette offered when creating a custom category. */
export const CATEGORY_COLORS = [
  '#2a78d6',
  '#eb6834',
  '#1baf7a',
  '#eda100',
  '#e87ba4',
  '#008300',
  '#4a3aa7',
  '#e34948',
] as const;

export const MONTH_NAMES = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December',
] as const;

/**
 * Emoji stand-ins for the seeded category icon keys. Keeps the first build free
 * of an icon-font dependency; swap for real icons later without touching data.
 */
export const CATEGORY_ICONS: Record<string, string> = {
  wallet: '💼',
  users: '🤝',
  'piggy-bank': '🐷',
  bus: '🚌',
  'heart-pulse': '🩺',
  receipt: '🧾',
  house: '🏠',
  utensils: '🍽️',
  'shopping-bag': '🛍️',
  film: '🎬',
  'graduation-cap': '🎓',
  wifi: '📶',
  'trending-up': '📈',
  ellipsis: '⋯',
};

export function iconFor(key: string): string {
  return CATEGORY_ICONS[key] ?? '•';
}
