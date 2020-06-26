using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Helpers
{
  public class SubscriptionHandler : AuthorizationHandler<SubscriptionRequirement>
  {
    public const string POLICY_NAME = "IsSubscribed";
    private readonly UserService _userService;

    public SubscriptionHandler(UserService userService)
    {
      _userService = userService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionRequirement requirement)
    {
      if (!context.User.HasClaim(x => x.Type == ClaimTypes.Expiration)) return Task.CompletedTask;

      if (!DateTime.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.Expiration).Value,
        out var dateSubscriptionEnd)) return Task.CompletedTask;
      
      if (DateTime.UtcNow < dateSubscriptionEnd)
        context.Succeed(requirement);
      
      context.Fail();

      return Task.CompletedTask;
    }
  }
  
  public class SubscriptionRequirement : IAuthorizationRequirement
  {
  }
}