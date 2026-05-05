using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var dto = request.UserDto;
        var user = await userRepository.GetByIdAsync(dto.UserId, ct);

        if (user is null || user.Account is null)
            return false;

        user.Update(dto.Firstname, dto.Lastname, dto.Patronymic, dto.Birthday, dto.Role);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
