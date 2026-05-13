using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

/// <summary>
/// The IUserService interface defines the contract for user-related operations in the application.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user in the system.
    /// The user object should contain necessary information such as email and password.
    /// The method will check if the email is already registered.
    /// The method will hash the password before saving it to the database.
    /// If registration is successful, it returns the registered user object;
    /// otherwise, it throws an exception with an appropriate error message.
    /// </summary>
    Task<User> Register(User? user);

    /// <summary>
    /// Retrieves a list of all registered users in the system.
    /// </summary>
    Task<List<User>> GetAllUsers();

    /// <summary>
    /// Retrieve a User from backend with that email and password.
    /// </summary>
    Task<User> Login(string email, string password);

    /// <summary>
    /// Retrive a User by his Email.
    /// </summary>
    Task<User> GetUserByEmail(string email);
}