using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Commands;

using FluentValidation;

namespace FinancialPlatform.Application.Validators;

public class SaveMarketDataCommandValidator : AbstractValidator<SaveMarketDataCommand>
{
    public SaveMarketDataCommandValidator()
    {
        RuleFor(x => x.MarketTick).NotNull().WithMessage("MarketTick cannot be null.");
        RuleFor(x => x.MarketTick.Ticker).NotEmpty().WithMessage("Ticker cannot be empty.");
        RuleFor(x => x.MarketTick.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
