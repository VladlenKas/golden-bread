using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

            if (await accountRepository.ExistsByEmailAsync(request.Email, null, ct))
                throw new DuplicateEntityException(nameof(request.Email));

            string passwordHash = passwordHasher.Create(request.Password);

            var account = DbEntities.Account.Create(
                0,
                request.Email,
                passwordHash,
                AccountType.User);

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
            await unitOfWork.CommitAsync(ct);

            return user.UserId;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}

