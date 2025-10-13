using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.Data;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.ShowTimeValidation
{
    public class ShowTimeUpdateDTOValidator : AbstractValidator<ShowTimeUpdateDTO>
    {
        private readonly AplicationDBContext _aplicationDBContext;
        public ShowTimeUpdateDTOValidator(AplicationDBContext aplicationDBContext)
        {
            _aplicationDBContext = aplicationDBContext;

            When(s => s.StartTime.HasValue, () =>
            {
                RuleFor(x => x.StartTime)
                    .GreaterThan(DateTime.Now)
                    .WithMessage("The session start time must be in the future.");
            });

            When(s => s.EndTime.HasValue, () =>
            {
                RuleFor(x => x.EndTime)
                    .GreaterThan(x => x.StartTime)
                    .WithMessage("The end time of the session must be later than the start time.");
            });

            When(s => s.ViewingFormat.HasValue, () =>
            {
                RuleFor(x => x.ViewingFormat)
                    .IsInEnum()
                    .WithMessage("Invalid viewing format.");
            });

            When(s => s.ShowTimeStatus.HasValue, () =>
            {
                RuleFor(s => s.ShowTimeStatus)
                    .IsInEnum()
                    .WithMessage("Invalid viewing format.");
            });

            When(s => s.TicketPrice.HasValue, () =>
            {
                RuleFor(s => s.TicketPrice)
                .GreaterThan(0)
                .WithMessage("The ticket price must be greater than 0.");
            });

            //RuleFor(x => x)
            //    .MustAsync(NoOverlappingShowTime)
            //    .WithMessage("There is already a showtime scheduled in the same time slot in the selected hall.");
        }

        //private async Task<bool> NoOverlappingShowTime(ShowTimeUpdateDTO dto, CancellationToken ct)
        //{
        //    return !await _aplicationDBContext.ShowTimes
        //                    .AnyAsync(st =>
        //                     st.HallId == dto.HallId &&
        //                     st.StartTime < dto.EndTime &&
        //                     st.EndTime > dto.StartTime &&
        //                    st.ShowTimeStatus == ShowTimeStatus.Active, ct);
        //}
    }
}

