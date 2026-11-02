# ExpressionCache

.NET library that builds cache keys from method call expressions (class name, method name and argument values) and caches the results. Two NuGet packages, both targeting net8.0, net9.0 and net10.0 and released together with the same version:

- `ExpressionCache.Core`: key generation (`ICacheKey`, `ICacheKeyBuilder`) and the provider model (`IExpressionCacheBase`, `IExpressionCacheProvider`, `ExpressionCacheBase`).
- `ExpressionCache.Distributed`: a provider for `IDistributedCache` (`IDistributedCacheService`, registered with `AddDistributedExpressionCache`).

## Workflow

- Conventional commits (`feat:`, `fix:`, `chore(deps):`, `ci:`, `build:`, `docs:`), one commit per concern, directly on `main`.
- Only push, tag, publish or edit GitHub releases when asked.
- Commit messages must be accurate. If one turns out to be wrong before it is pushed, reword it (replay the commits and check that the tree is unchanged). Never rewrite pushed history.

## Releasing

- **NuGet packages:** push a lightweight tag `vX.Y.Z` on `main`. CI publishes to NuGet on tags. Every push to `main` also publishes a beta to MyGet.
- **Verifying:** NuGet serves a new package for download after about 3 minutes; if a restore doesn't find it yet, clear the cache with `dotnet nuget locals http-cache --clear` and retry. After publishing, install the package from the registry and build a project that uses it.
- **Versioning:** any change to a public interface (`IExpressionCacheBase`, `IExpressionCacheProvider`, `ICacheKey`, `ICacheKeyBuilder`, `IDistributedCacheService`, `AddDistributedExpressionCache`) is breaking and needs a major version.

### GitHub release notes

Use these sections, omitting empty ones, and end with a compare link:

```markdown
## Breaking changes

## Features

## Fixes

## Known issues

## Other

**Full Changelog**: https://github.com/Baune8D/ExpressionCache/compare/vA...vB
```

- Write for users of the package: name the exact APIs, settings and before/after values. Leave out internal changes (tests, CI, IDE files).
- When a release fixes a bug from an earlier release, add a "Known issues" entry to the earlier release saying "Fixed in vX.Y.Z".

## Dependencies

- Dependabot runs weekly on Monday with a 7-day cooldown. Minor/patch updates are grouped; security updates have their own group.
- Major versions are ignored by Dependabot and upgraded by hand: read the release notes, update, and build/test before committing. If a major breaks the build and fixing it is a design decision, keep the old major and explain why in the commit.
- New exceptions go in `.github/dependabot.yml` with a comment explaining why.
- Before holding a package back, find the root cause and check the package's issue tracker.
- `Directory.Packages.props` gives the Microsoft.Extensions packages a version range per target framework (`[8,9)` for net8.0 and so on). The lower bound is the lowest version the library supports, so Dependabot ignores them; raise it by hand when the library needs a newer version. NuGet restores the lowest version in a range, so the tests run against the lowest supported versions.
- Dependabot restores each target framework separately in projects with more than two, so the lock files in its NuGet PRs only keep the last one. `dependabot-lock-files.yml` regenerates them, pushes the fix with `[skip ci]` and starts the pipeline, which reports a `pipeline` commit status on the PR. Dependabot doesn't rebase a PR that has someone else's commit; comment `@dependabot recreate` to update one.
- `ExpressionCache.Distributed.Tests` references Newtonsoft.Json directly. The tests use it, and Microsoft.NET.Test.Sdk 18.11 stopped bringing it in.
- `build/_build.csproj` pins `NuGet.Packaging` and `System.Security.Cryptography.Xml` to patch vulnerable dependencies of Nuke.Common (NuGet 7.9 also fixes a `NuGet.Frameworks` load error with SDK 10.0.4xx). Remove the pins once Nuke.Common ships fixed versions.

## Building and testing

- `./build.sh Test` builds and runs the tests for all target frameworks. This machine only has the .NET 10 runtime; use `DOTNET_ROLL_FORWARD=Major ./build.sh Test` to run the net8.0/net9.0 tests on it. CI has all three runtimes.
- `global.json` is required: DotNet.ReproducibleBuilds 2.x warns without it, and the build treats warnings as errors.
- `TreatWarningsAsErrors` also turns NuGet's vulnerability warnings (NU1901 to NU1904) into errors, so a newly published advisory for any dependency, direct or transitive, fails the restore (in CI too) until the package is updated or overridden.
- Restores use the lock files (`packages.lock.json`) in locked mode. After changing a package, run `dotnet restore ExpressionCache.slnx --force-evaluate` and commit the lock files.

## README

The root `README.md` is shown on GitHub. Each package packs its own `src/<Package>/README.md` into the NuGet package, so keep both in sync when the usage changes. Use absolute links and no raw HTML in them, since nuget.org doesn't render either.
