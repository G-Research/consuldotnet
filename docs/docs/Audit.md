# [Consul.NET](https://github.com/G-Research/consuldotnet) Maturity Audit

(Based on RepoLinter Summary and Community Documentation Review)

---

## Overview

This audit was conducted to assess **Consul.NET’s** alignment with open-source best practices and maturity standards using repolinter and a manual community docs check.

---

### Missing / Needs Improvement

| Category  | Issue                        | Description                                                                                        | Recommended Action                     |
| --------- | ---------------------------- | -------------------------------------------------------------------------------------------------- | -------------------------------------- |
| Security  | security-file doesn't exist  | No security disclosure policy                                                                      | Add `.github/SECURITY.md`              |
| Glossary  | No Glossary.md file          | No glossary file to help define project specific terms or acronyms in an accessible location       | Add `Glossary.md`                      |
| Adopters  | No Adopters.md file          | No adopters file to provide a way so teams and users can self-identify as adopters of the project. | Add `Adopters.md`                      |
| Community | github-pull-request-template | Missing PR template                                                                                | Add `.github/pull_request_template.md` |
| Community | github-issue-template        | Missing issue templates                                                                            | Add `.github/ISSUE_TEMPLATE/*.md`      |

---

## Notes

- It's included here that the repo lacks a github pull request template and issue template according to repolinter. However, templates for PRs and issues appear to rely on default github issue and PR forms which works like a template.

- The setup and contributor documentation could benefit from clearer differentiation between _users_ (NuGet consumers) and _contributors_.

- Some files that appear missing based on the community docs check are actually present but with a different name format or paths.

- RepoLinter flagged several missing files that do exist in the repository but under slightly different names or locations (e.g. test directory, changelog, etc).

---
