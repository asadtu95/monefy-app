import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';

import { CurrencyFormatPipe } from './currency-format-pipe';
import { Auth } from '../../core/services/auth';

describe('CurrencyFormatPipe', () => {
  const profile = signal<{ currency: string } | null>(null);

  function create(): CurrencyFormatPipe {
    TestBed.configureTestingModule({
      providers: [{ provide: Auth, useValue: { profile } }],
    });
    return TestBed.runInInjectionContext(() => new CurrencyFormatPipe());
  }

  beforeEach(() => profile.set(null));

  it('falls back to rupees before the profile loads', () => {
    expect(create().transform(1500)).toBe('₨1,500');
  });

  it('uses the currency chosen in settings', () => {
    const pipe = create();
    profile.set({ currency: 'USD' });
    expect(pipe.transform(1500)).toBe('$1,500');
  });

  it('puts the minus sign before the symbol', () => {
    expect(create().transform(-250)).toBe('-₨250');
  });

  it('compacts large numbers', () => {
    const pipe = create();
    expect(pipe.transform(12_900, 'compact')).toBe('₨12.9K');
    expect(pipe.transform(-4_200_000, 'compact')).toBe('-₨4.2M');
  });
});
