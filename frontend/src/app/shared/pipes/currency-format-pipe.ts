import { Pipe, PipeTransform } from '@angular/core';

/**
 * Money formatter. `compact` gives 12.9K / 4.2M for stat tiles and axis ticks;
 * the default gives the full grouped number for lists and totals.
 */
@Pipe({
  name: 'currencyFormat',
})
export class CurrencyFormatPipe implements PipeTransform {
  private static readonly SYMBOL = '₨';

  transform(value: number | null | undefined, mode: 'full' | 'compact' = 'full'): string {
    const amount = value ?? 0;

    if (mode === 'compact') {
      return `${CurrencyFormatPipe.SYMBOL}${this.compact(amount)}`;
    }

    return `${CurrencyFormatPipe.SYMBOL}${amount.toLocaleString('en-US', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    })}`;
  }

  private compact(amount: number): string {
    const sign = amount < 0 ? '-' : '';
    const abs = Math.abs(amount);

    if (abs >= 1_000_000) {
      return `${sign}${this.trim(abs / 1_000_000)}M`;
    }
    if (abs >= 1_000) {
      return `${sign}${this.trim(abs / 1_000)}K`;
    }

    return `${sign}${this.trim(abs)}`;
  }

  private trim(n: number): string {
    return n.toFixed(1).replace(/\.0$/, '');
  }
}
