import { Pipe, PipeTransform, inject } from '@angular/core';

import { Auth } from '../../core/services/auth';
import { SUPPORTED_CURRENCIES } from '../../core/constants/app.constants';

const DEFAULT_SYMBOL = '₨';

/**
 * Money formatter using the currency chosen in Settings. `compact` gives
 * 12.9K / 4.2M for stat tiles and axis ticks; the default gives the full
 * grouped number for lists and totals.
 *
 * Impure so amounts re-render when the currency changes; the work per call is
 * a lookup and a toLocaleString, which is cheap.
 */
@Pipe({
  name: 'currencyFormat',
  pure: false,
})
export class CurrencyFormatPipe implements PipeTransform {
  private readonly auth = inject(Auth);

  transform(value: number | null | undefined, mode: 'full' | 'compact' = 'full'): string {
    const amount = value ?? 0;
    const symbol = this.symbol();
    const sign = amount < 0 ? '-' : '';
    const abs = Math.abs(amount);

    if (mode === 'compact') {
      return `${sign}${symbol}${this.compact(abs)}`;
    }

    return `${sign}${symbol}${abs.toLocaleString('en-US', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    })}`;
  }

  private symbol(): string {
    const code = this.auth.profile()?.currency;
    return SUPPORTED_CURRENCIES.find((c) => c.code === code)?.symbol ?? DEFAULT_SYMBOL;
  }

  private compact(abs: number): string {
    if (abs >= 1_000_000) {
      return `${this.trim(abs / 1_000_000)}M`;
    }
    if (abs >= 1_000) {
      return `${this.trim(abs / 1_000)}K`;
    }

    return this.trim(abs);
  }

  private trim(n: number): string {
    return n.toFixed(1).replace(/\.0$/, '');
  }
}
