# NgFate - Angular Project Report

A tool for deciding the fate of an Angular project. This will help you to find the modules, components relation and hirarchy.

Algorithm:

1. get all '.module.ts' files [Done]
1. get all 'declarations' array data from NgModules [Done]
1. decompose fileName based on camelCasing of name [Done]
1. get file path by search fileName in the directory [Done]
1. read '*-routing.module.ts' file and search name of component if existed [Done]
1. get the path name while searching for component in routing file [Done]
1. search for 'header.component.html' file in entire project and get the .html parent files [Done]
1. decompose fileName from name [Done]

Data Structure:

```
[
  // 1. get all '.module.ts' files
  {
    name: 'AppModule',
    fileName: 'app.module.ts',
    components: [
      // 2. get all 'declarations' array data from NgModules
      {
        name: 'LoginComponent',
        fileName: 'login.component.ts', // 3. decompose fileName based on camelCasing of name
        filePath: 'src/app/pages/login.component.ts', // 4. get file path by search fileName in the directory
        routed: true, // 5. read '*-routing.module.ts' file and search name of component if existed
        routePath: 'login', // 6. get the path name while searching for component in routing file
        parents: [],
      },
      {
        name: 'HeaderComponent',
        fileName: 'header.component.ts',
        filePath: 'src/app/components/header.component.ts',
        routed: false, // 5. read '*-routing.module.ts' file and search name of component if existed
        routePath: null,
        parents: [
          // 7. search for 'header.component.html' file in entire project and get the .html parent files
          {
            name: 'MainComponent',
            fileName: 'main.component.ts', // 8. decompose fileName from name
            filePath: 'src/app/components/main.component.ts', // 4. get file path by search fileName in the directory
          },
        ],
      },
    ],
  },
];
```

Todo:

- Exception handling
- Optimize app performance