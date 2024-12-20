//using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPIMoviez.EndPoints
{
    public static class UserEndpoints
    {
        public static RouteGroupBuilder MapUser(this RouteGroupBuilder group)
        {
            group.MapPost("/Register", Register).AddEndpointFilter<ValidationFilter<UserCredentialsDTO>>();
            group.MapPost("/Login", Login).AddEndpointFilter<ValidationFilter<UserCredentialsDTO>>();
            return group;
        }
        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<string>>> 
            Login(UserCredentialsDTO userCredentialsDTO,
            [FromServices] UserManager<IdentityUser> userManager, 
            [FromServices] SignInManager<IdentityUser> signInManager,
            IConfiguration configuration
            )
        {
            // check wether user exists
            var user = await userManager.FindByEmailAsync(userCredentialsDTO.Email);
            if (user == null)
            {
                return TypedResults.BadRequest("There was a Problem with Username or Password");
            }

            var result = await signInManager.
                            CheckPasswordSignInAsync(user, userCredentialsDTO.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var authenticatonResponse = await TokenBuilder(userCredentialsDTO, configuration, userManager);
                return TypedResults.Ok(authenticatonResponse);
            }
            else
            {
                return TypedResults.BadRequest("There was an error with Username or Password");
            }
        }

        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<IEnumerable<IdentityError>>>> Register(UserCredentialsDTO userCredentialsDTO,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialsDTO.Email,
                Email = userCredentialsDTO.Email
            };

            // try to create user in the database with the provided password
            var result = await userManager.CreateAsync(user, userCredentialsDTO.Password);

            if (result.Succeeded) // if creation successful 
            {
                // generate a JWT token for user using TokenBuilder()
                var authenticatonResponse = await TokenBuilder(userCredentialsDTO, configuration, userManager);
                return TypedResults.Ok(authenticatonResponse);

            }
            else
            {
                return TypedResults.BadRequest(result.Errors);
            }
        }
        // Generates a JWT token for the authenticated user
        private async static Task<AuthenticationResponseDTO> TokenBuilder(UserCredentialsDTO userCredentialsDTO,
            IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            var claimList = new List<Claim>
            {
                new Claim("Email", userCredentialsDTO.Email)
             // new Claim(What we need, value we give)
            };

         // check for existense of Email in DB
            var user = await userManager.FindByNameAsync(userCredentialsDTO.Email);
            //get additional information
            var claimsFromDB = await userManager.GetClaimsAsync(user!);

         // add additional info from DB about users(eg. accesslevel,...) to our claimList
            claimList.AddRange(claimsFromDB);

         // Get Key from configuration and store it using HMAC 256
            var key = KeysHandler.GetKey(configuration).First();
            var credential = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMonths(6);

            var securityToken = new JwtSecurityToken(issuer:null,audience:null, claims: claimList,
                expires: expiration,signingCredentials: credential);

         //  convert security token into a string format JWT
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO
            {
                Token = token,
                ExpireDate = expiration
            };
        }

    }
}
