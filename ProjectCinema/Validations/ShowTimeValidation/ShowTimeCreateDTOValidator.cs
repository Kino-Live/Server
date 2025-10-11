using FluentValidation;
using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.BLL.Interfaces.IMovieScreeningServices;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.Data;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.ShowTimeValidation
{
    public class ShowTimeCreateDTOValidator : AbstractValidator<ShowTimeCreateDTO>
    {
        // Сервисы/репозитории, необходимые для проверки бизнес-правил.
        private readonly IHallService _hallService;
        private readonly IMovieScreeningCrudService _movieScreeningCrudService;
        private readonly AplicationDBContext _aplicationDBContext;

        public ShowTimeCreateDTOValidator(IHallService hallService,
                                          IMovieScreeningCrudService movieScreeningCrudService,
                                          AplicationDBContext aplicationDBContext
                                          )
        {
            _hallService = hallService;
            _movieScreeningCrudService = movieScreeningCrudService;
            _aplicationDBContext = aplicationDBContext;


            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start time is a required field")
                .GreaterThan(DateTime.Now)
                .WithMessage("The session start time must be in the future.");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("End time is a required field")
                .GreaterThan(x => x.StartTime)
                .WithMessage("The end time of the session must be later than the start time.");

            RuleFor(x => x.TicketPrice)
                .NotEmpty()
                .WithMessage("Ticket price is a required field")
                .GreaterThan(0)
                .WithMessage("The ticket price must be greater than 0.");

            RuleFor(x => x.HallId)
                .NotEmpty()
                .WithMessage("Hall id is a required field")
                .MustAsync(async (hallId, cancellation) =>
                {
                    var hall = await _hallService.GetByIdAsync(hallId);
                    return hall != null;
                })
                .WithMessage(dto => $"Hall id equal {dto.HallId} does not exist");

            RuleFor(x => x.MovieScreeningId)
                .NotEmpty()
                .WithMessage("Movie screening is a required field")
                .MustAsync(async (movieScreeningId, cancellation) =>
                {
                    var screening = await _movieScreeningCrudService.GetByIdAsync(movieScreeningId);
                    return screening != null;
                })
                .WithMessage(dto => $"MovieScreening id equal {dto.MovieScreeningId} does not exist");

            RuleFor(x => x)
                .MustAsync(NoOverlappingShowTime)
                .WithMessage("There is already a showtime scheduled in the same time slot in the selected hall.");

            RuleFor(x => x.ViewingFormat)
                .IsInEnum()
                .WithMessage("Invalid viewing format.");

        }

        private async Task<bool> NoOverlappingShowTime(ShowTimeCreateDTO dto, CancellationToken ct)
        {
            return !await _aplicationDBContext.ShowTimes
                .AnyAsync(st =>
                    st.HallId == dto.HallId &&
                    st.StartTime < dto.EndTime &&
                    st.EndTime > dto.StartTime &&
                    st.ShowTimeStatus == ShowTimeStatus.Active, ct);
        }
    }
}
