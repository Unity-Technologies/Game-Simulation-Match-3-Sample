# Template repository for new Microgames

1. Create a new repository using this template repository.
1. In your new repository, rename the following files and/or their contents to match your product's name:
    - `Assets/NAME`
    - `Packages/com.unity.template.NAME` (**must use lowercase name as package's name**)
    - `Packages/com.unity.template.NAME/package.json` (**must use lowercase name as package's name**)
    - `ProjectSettings/ProjectSettings.asset`:
        ```
          applicationIdentifier:
            Standalone: com.unity.template.microgamename
        ```
    - `Packages/com.unity.template.NAME/LICENSE.md`
    - `Packages/com.unity.template.NAME/CHANGELOG.md`
    - `Packages/com.unity.template.NAME/README.md`
    - `Packages/com.unity.template.NAME/Documentation~/your-template-name.md`
    - search for files matching `Unity.NAME.*` and rename `NAME` to match the Microgame's name
    - in code, search matching `Unity.NAME` namespaces and rename `NAME` to match the Microgame's name
1. Open the project and validate everything appears to work:
    - run unit tests
    - run Package Validation Suite
    - make a WebGL build
1. Set up Yamato for your project:
    - https://internaldocs.hq.unity3d.com/yamato/workflows/quick-start/
    - Find _Microgame Template Repository_ in Yamato and copy its _Settings_ (Gear icon) > _Other Settings_ >
    _Subscriptions_ in order to have CI notifications for the project on Slack.
1. Set up branch protection rules for `master`, `dev`, `dev-iet`, and `staging`.
