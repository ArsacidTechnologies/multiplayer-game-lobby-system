extern alias PlayerServiceAlias;
using Microsoft.AspNetCore.Mvc.Testing;


namespace test_fixtures
{
    public class PlayerServiceFixture : WebApplicationFactory<PlayerServiceAlias::Program> { }
}