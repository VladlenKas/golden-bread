using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var dto = request.UserDto;
            string passwordHash = passwordHasher.Create(request.Password);

            var account = DbEntities.Account.Create(
                0,
                request.Email,
                passwordHash,
                dto.AccountType);

            await accountRepository.AddAsync(account, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var user = User.Create(
                0,
                account.AccountId,
                dto.Lastname,
                dto.Firstname,
                dto.Patronymic,
                dto.Birthday,
                dto.Role);

            await userRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return user.UserId;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}

