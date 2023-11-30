# Uagc.Vdc.Versioning.Library

... based on Semantic Versioning (https://semver.org)

# Vocabulary
<details>
<summary><i>Click to expand/collapse...</i></summary>

| Term | Meaning |
| ---- | ------- |
| 3 section Version | Frex, "1.2.3". |
| IT Asset | A developed or purchased codebase intended to serve multiple users that will run in Production. |
| Breaking change | A change to an asset (config, code, etc.) is considered breaking if anything interfacing with that asset must change to keep working. |
| Non-breaking change | Asset change that does not affect any interfacing systems, usually a bug fix, config change, or a feature addition. |
| Changelog | Changelog is a feature of the Versioning library that allows developers to annotate and increment Major, Minor, or Patch each time they make a change to the asset.  This forms a running history of version changes that roughly track code commits and Pull Requests. |
| Development Version | 0.x.x - by convention, Major is not incremented until a project is submitted to QA. |
| Durable | A thing that is persisted between uses, such as a Form in AllForms.   |
| Durable Entity | A major program entity that is placed in permanent storage and retrieved or copied for use.  A Durable Entity serves many sessions, and survives crashes and restarts. |
| Content/Cosmetics | Changes to document text or other content, or changes that affect appearance but not function. |
| Major | The first section in the Version number. It is incremented when a breaking change, such as a bug fix or feature change, affects the asset's API.  When Major is incremented, Minor and Patch are zeroed. |
| Minor | The second section in the Version number, incrementing when a non-breaking change is introduced. Incrementing Minor zeroes Patch. |
| Patch | The third section in the Version number, incrementing when a cosmetic or content change has been made, which by their nature should be non-breaking. |
| Semantic Versioning | http:://semver.org  An excellent overview of Semantic Versioning, which our system imitates. |

</details>

## What needs to be versioned?
- <span style="color:springgreen">IT assets</span> (apps, services, interfaces, code libraries, etc.)... 
  - that are or will be deployed in Production; or...
  - made available for use in production code
- <span style="color:springgreen">Durable entities</span>, code elements that persist across user sessions and application restarts.  Templates (such as communication frameworks saved in Managed Templates) and sometimes configuration files will benefit from versioning.
