using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.Row;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Data;

namespace ProjectCinema.Validations.RowValidation
{
    public class RowUpdateDTOValidator : AbstractValidator<RowUpdateDTO>
    {
        private readonly IHallService _hallService;
        private readonly AplicationDBContext _context;
        public RowUpdateDTOValidator(IHallService hallService, AplicationDBContext context)
        {
            _hallService = hallService;
            _context = context;


            When(r => r.RowNumber.HasValue, () =>
            {
                RuleFor(r => r.RowNumber)
                    .GreaterThan(0).WithMessage("Row number must be a positive number");

                    //.MustAsync(async (dto, rowNumber, cancellation) =>
                    //{
                    //    var hall = await _hallService.GetByIdAsync(dto.HallId);
                    //    return rowNumber <= hall.RowCount;
                    //}).WithMessage("Row number exceeds the total number of rows allowed in the hall")

                    //.MustAsync(async (dto, rowNumber, cancellation) =>
                    //{
                    //    return !await _context.Rows
                    //    .AnyAsync(r => r.HallId == dto.HallId && r.RowNumber == rowNumber, cancellation);
                    //}).WithMessage("Row number must be unique within the hall");
            });
        }
    }
}
