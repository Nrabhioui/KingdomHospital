1\. Description
---------------

KindomHospital est une API REST ASP.NET Core (.NET 9) pour la gestion d’un cabinet / hôpital :spécialités, médecins, patients, consultations, médicaments, ordonnances et lignes d’ordonnance.Le projet suit la **Clean Architecture** : Presentation → Application → Domain → Infrastructure.

2\. Structure du projet
-----------------------

Racine du projet : KindomHospital/

*   Presentation/
    
    *   Controllers/
        
        *   SpecialtiesController.cs
            
        *   DoctorsController.cs
            
        *   PatientsController.cs
            
        *   ConsultationsController.cs
            
        *   MedicamentsController.cs
            
        *   OrdonnancesController.cs
            
*   Application/
    
    *   DTOs/
        
        *   Specialties/ (SpecialtyDto, SpecialtyCreateDto, SpecialtyUpdateDto, …)
            
        *   Doctors/
            
        *   Patients/
            
        *   Consultations/
            
        *   Medicaments/
            
        *   Ordonnances/
            
        *   OrdonnanceLignes/
            
    *   Mappers/ (Mapperly)
        
        *   SpecialtyMapper, DoctorMapper, PatientMapper, ConsultationMapper,MedicamentMapper, OrdonnanceMapper, OrdonnanceLigneMapper
            
    *   Services/
        
        *   SpecialtyService, DoctorService, PatientService,ConsultationService, MedicamentService,OrdonnanceService, OrdonnanceLigneService
            
*   Domain/
    
    *   Entities/
        
        *   Specialty, Doctor, Patient, Consultation,Medicament, Ordonnance, OrdonnanceLigne (modélisation clinique)
            
*   Infrastructure/
    
    *   HospitalDbContext.cs
        
    *   Configurations/ (Fluent API EF Core)
        
    *   Migrations/ (migrations EF Core)
        
    *   Repositories/ (accès aux données, si utilisé)
        
    *   SeedData.cs (initialisation de données)
        
    *   Data/
        
        *   specialties.csv (Annexe 1 du sujet)
            
        *   medicaments.csv (Annexe 2 du sujet)
            
*   Fichiers racine
    
    *   Program.cs (configuration de l’API, DI, Serilog, DbContext, Seed)
        
    *   appsettings.json / appsettings.Development.json
        
    *   KindomHospital.http (jeu de tests HTTP)
        

3\. Prérequis
-------------

*   .NET 9 SDK
    
*   SQL Server LocalDB (ou instance équivalente)
    
*   Visual Studio 2025 / Rider
    
*   Connection string (dans appsettings.json) :
    

Plain textANTLR4BashCC#CSSCoffeeScriptCMakeDartDjangoDockerEJSErlangGitGoGraphQLGroovyHTMLJavaJavaScriptJSONJSXKotlinLaTeXLessLuaMakefileMarkdownMATLABMarkupObjective-CPerlPHPPowerShell.propertiesProtocol BuffersPythonRRubySass (Sass)Sass (Scss)SchemeSQLShellSwiftSVGTSXTypeScriptWebAssemblyYAMLXML`   "ConnectionStrings": {    "HospitalDb": "Server=(localdb)\\MSSQLLocalDB;Database=KingdomHospitalDb;Trusted_Connection=True;TrustServerCertificate=True;"  }   `

4\. Configuration & démarrage
-----------------------------

1.  **Restaure les packages NuGet** (Visual Studio : Rebuild du projet).
    
2.  Vérifie la **chaîne de connexion** dans appsettings.json.
    
3.  Add-Migration InitialCreate -OutputDir Infrastructure/MigrationsUpdate-Database
    
    *   Default project = projet Web (KindomHospital)
        
    *   Commandes :
        
4.  dotnet run --project KindomHospital/KindomHospital.csprojL’API écoute sur les URLs configurées dans launchSettings.json(par ex. https://localhost:7006 et http://localhost:5039).
    
5.  Documentation OpenAPI :
    
    *   Document : /openapi/v1.json
        
    *   UI (Scalar/Swagger) si activée dans le projet.
        

5\. Seed des données
--------------------

À chaque démarrage, Program.cs appelle :

Plain textANTLR4BashCC#CSSCoffeeScriptCMakeDartDjangoDockerEJSErlangGitGoGraphQLGroovyHTMLJavaJavaScriptJSONJSXKotlinLaTeXLessLuaMakefileMarkdownMATLABMarkupObjective-CPerlPHPPowerShell.propertiesProtocol BuffersPythonRRubySass (Sass)Sass (Scss)SchemeSQLShellSwiftSVGTSXTypeScriptWebAssemblyYAMLXML`   using (var scope = app.Services.CreateScope())  {      var context = scope.ServiceProvider.GetRequiredService();      SeedData.Initialize(context);  }   `

SeedData.Initialize :

1.  **Specialties**
    
    *   Lit Data/specialties.csv (Annexe 1 du sujet).
        
    *   Ignore les doublons, tronque à 30 caractères, insère si la table est vide.
        
2.  **Medicaments**
    
    *   Lit Data/medicaments.csv (Annexe 2).
        
    *   Respecte Name <= 100, DosageForm/Strength <= 30, AtcCode <= 20.
        
3.  **Données de démo** (si tables vides)
    
    *   6 médecins répartis dans plusieurs spécialités.
        
    *   5 patients (dates de naissance plausibles).
        
    *   10 consultations (0–3 par patient, au moins un patient sans consultation,au moins une journée avec 2 consultations du même médecin à des heures différentes).
        
    *   5 ordonnances (dont une avec 3 médicaments et un patient avec ≥ 2 ordonnances).
        
    *   Lignes d’ordonnances cohérentes avec les contraintes de l’énoncé.
        

6\. Endpoints principaux (CRUD)
-------------------------------

Résumé des endpoints implémentés, en phase avec la section 9 du sujet.

### Specialties

*   GET /api/specialties – liste des spécialités
    
*   GET /api/specialties/{id} – détail d’une spécialité
    

### Doctors

*   GET /api/doctors
    
*   GET /api/doctors/{id}
    
*   POST /api/doctors
    
*   PUT /api/doctors/{id}
    
*   (Optionnel) DELETE /api/doctors/{id} avec règles métier.
    

### Patients

*   GET /api/patients
    
*   GET /api/patients/{id}
    
*   POST /api/patients
    
*   PUT /api/patients/{id}
    
*   DELETE /api/patients/{id}
    

### Consultations

*   GET /api/consultations
    
*   GET /api/consultations/{id}
    
*   POST /api/consultations
    
*   PUT /api/consultations/{id}
    
*   DELETE /api/consultations/{id}
    

### Medicaments

*   GET /api/medicaments
    
*   GET /api/medicaments/{id}
    

### Ordonnances & Lignes

*   GET /api/ordonnances
    
*   GET /api/ordonnances/{id}
    
*   POST /api/ordonnances
    
*   PUT /api/ordonnances/{id}
    
*   DELETE /api/ordonnances/{id}
    
*   GET /api/ordonnances/{id}/lignes
    
*   POST /api/ordonnances/{id}/lignes
    
*   GET /api/ordonnances/{id}/lignes/{ligneId}
    
*   PUT /api/ordonnances/{id}/lignes/{ligneId}
    
*   DELETE /api/ordonnances/{id}/lignes/{ligneId}
    

7\. Endpoints relationnels & utilitaires
----------------------------------------

### Relationnels

*   **Doctor ↔ Specialty**
    
    *   GET /api/specialties/{id}/doctors
        
    *   GET /api/doctors/{id}/specialty
        
    *   PUT /api/doctors/{id}/specialty/{specialtyId}
        
*   **Doctor ↔ Consultations / Patients / Ordonnances**
    
    *   GET /api/doctors/{id}/consultations
        
    *   GET /api/doctors/{id}/patients
        
    *   GET /api/doctors/{id}/ordonnances
        
*   **Patient ↔ Consultations / Ordonnances**
    
    *   GET /api/patients/{id}/consultations
        
    *   GET /api/patients/{id}/ordonnances
        
*   **Consultations ↔ Ordonnances**
    
    *   GET /api/consultations/{id}/ordonnances
        
    *   POST /api/consultations/{id}/ordonnances
        
    *   PUT /api/ordonnances/{id}/consultation/{consultationId}
        
    *   DELETE /api/ordonnances/{id}/consultation
        
*   **Médicament ↔ Ordonnances**
    
    *   GET /api/medicaments/{id}/ordonnances
        

### Endpoints utilitaires

Conformes à la section _Endpoints utilitaires_ du sujet.

*   GET /api/consultations?doctorId=&patientId=&from=&to=
    
*   GET /api/ordonnances?doctorId=&patientId=&from=&to=
    

> Règle : doctorId et patientId peuvent être null, mais au moins un doit être renseigné.

8\. Jeu de tests (.http)
------------------------

Le fichier **KindomHospital.http** contient des scénarios de test comme demandé :

*   Lecture initiale (**GET avant**)
    
*   POST / PUT / DELETE sur les endpoints CRUD et relationnels
    
*   Lecture finale (**GET après**) pour visualiser l’impact
    

Ce fichier peut être exécuté dans Visual Studio (HTTP Client) ou importé en Postman via l’OpenAPI.

9\. Technologies
----------------

*   ASP.NET Core Web API (.NET 9)
    
*   Entity Framework Core (Code First, SQL Server)
    
*   Mapperly (mapping entités ↔ DTO)
    
*   Serilog (logging console + fichier)
    
*   Clean Architecture (Presentation / Application / Domain / Infrastructure)