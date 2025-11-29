# Changelog

All notable changes to this project will be documented in this file.


## Other

- Initial commit: PipeException NuGet library with Nuke build system

- Add PipeException validation library with pipe operator syntax
- Set up Nuke 10.0.0 build system with Clean, Restore, Compile, Test, Pack, Push targets
- Configure MinVer for automatic versioning from git tags
- Add Meziantou.Analyzer for code quality validation
- Configure GitHub Actions CI/CD (build on PR, publish on tags)
- Add SourceLink for debugging support
- Include symbol packages (.snupkg)
- Add README, LICENSE (MIT), and documentation

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com> (1a2e992)
- Fix GitHub Actions: use build.sh and add .NET 10 setup

- Change ./build.cmd to ./build.sh for Ubuntu runners
- Add setup-dotnet step with .NET 10.0.x preview

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com> (6cc1eee)
- Add renovate.json (623b35d)
- Merge pull request #1 from phmatray/renovate/configure

Configure Renovate (0670b5e)
- Update dependency Meziantou.Analyzer to 2.0.257 (e16b5f5)
- Merge pull request #2 from phmatray/renovate/meziantou.analyzer-2.x

Update dependency Meziantou.Analyzer to 2.0.257 (a2a8c94)
- Update actions/checkout action to v6 (f4d7b0d)
- Merge pull request #3 from phmatray/renovate/actions-checkout-6.x

Update actions/checkout action to v6 (b434b1e)
- Update SDK version to 10.0.100

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com> (b54f9d2)
- Update actions/upload-artifact action to v5 (423e703)
- Merge pull request #6 from phmatray/renovate/major-github-artifact-actions

Update actions/upload-artifact action to v5 (0c0e5c9)
- Update actions/setup-dotnet action to v5 (8dae01d)
- Merge pull request #5 from phmatray/renovate/actions-setup-dotnet-5.x

Update actions/setup-dotnet action to v5 (9f26a4e)
- Merge remote-tracking branch 'origin/main' (60f3057)
- Fix CS8618 nullable warnings in Build.cs

Make NuGetApiKey and Solution fields nullable since they are
populated at runtime by Nuke attributes.

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com> (dcda318)
- Add logo to NuGet package and README

- Include Logo.png in NuGet package with PackageIcon property
- Add centered logo to README header
- Resize logo from 1024x1024 (1.5MB) to 512x512 (304KB)

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com> (8018905)


