# 🏥 KingdomHospital – API .NET 9  
API de gestion hospitalière basée sur **Clean Architecture**, **Entity Framework Core**, **Mapperly** et **SQL Server**.

Ce projet implémente la gestion complète des éléments suivants :
- Spécialités médicales  
- Médecins  
- Patients  
- Consultations  
- Ordonnances  
- Lignes d’ordonnance  
- Catalogue de médicaments  

Le tout à travers une API RESTful propre, testable, documentée et conforme au cahier des charges.

---

## 🚀 Technologies utilisées

- **.NET 9 Web API**
- **Entity Framework Core 9**
- **SQL Server (LocalDB)**
- **Mapperly** *(génération automatique de mappers compile-time)*
- **Serilog** *(logging)*
- **Clean Architecture** *(séparation stricte des responsabilités)*

---

## 🧱 Architecture du projet

Le projet suit strictement la Clean Architecture :

┌──────────────────────────────┐
│ Presentation │ → Controllers API
└──────────────▲───────────────┘
│
┌──────────────┴───────────────┐
│ Application │ → Services, DTOs, Mapperly
└──────────────▲───────────────┘
│
┌──────────────┴───────────────┐
│ Domain │ → Entités métier (POCO)
└──────────────▲───────────────┘
│
┌──────────────┴───────────────┐
│ Infrastructure │ → EF Core, DB, Configurations, Seed
└──────────────────────────────┘


### ➜ **Presentation**
Contient uniquement les **Controllers**.  
Reçoit les requêtes HTTP, déclenche les services Application, retourne les DTOs.

### ➜ **Application**
Contient :
- DTOs
- Services (logique métier applicative)
- Mappers (Mapperly)
Aucune dépendance vers EF Core.

### ➜ **Domain**
Contient **uniquement les entités métier**.

### ➜ **Infrastructure**
Contient :
- DbContext EF Core  
- Configurations (IEntityTypeConfiguration)  
- Migrations  
- SeedData  
- Repositories (implémentation des accès aux données)

# 📁 Structure complète du projet

Ce projet suit une organisation claire selon la Clean Architecture.  
Voici l’arborescence complète :

C:.
|   appsettings.Development.json
|   appsettings.json
|   arborescence.txt
|   KingdomHospital.csproj
|   KingdomHospital.csproj.user
|   KingdomHospital.http
|   Program.cs
|   README - Copy.md
|   README.md
|   
+---Application
|   +---DTOs
|   |   +---Consultations
|   |   |       ConsultationCreateDto.cs
|   |   |       ConsultationDto.cs
|   |   |       ConsultationUpdateDto.cs
|   |   |       
|   |   +---Doctors
|   |   |       DoctorCreateDto.cs
|   |   |       DoctorDto.cs
|   |   |       DoctorUpdateDto.cs
|   |   |       
|   |   +---Medicaments
|   |   |       MedicamentCreateDto.cs
|   |   |       MedicamentDto.cs
|   |   |       MedicamentUpdateDto.cs
|   |   |       
|   |   +---OrdonnanceLignes
|   |   |       OrdonnanceLigneCreateDto.cs
|   |   |       OrdonnanceLigneDto.cs
|   |   |       OrdonnanceLigneUpdateDto.cs
|   |   |       
|   |   +---Ordonnances
|   |   |       OrdonnanceCreateDto.cs
|   |   |       OrdonnanceDto.cs
|   |   |       OrdonnanceUpdateDto.cs
|   |   |       
|   |   +---Patients
|   |   |       PatientCreateDto.cs
|   |   |       PatientDto.cs
|   |   |       PatientUpdateDto.cs
|   |   |       
|   |   \---Specialties
|   |           SpecialtyCreateDto.cs
|   |           SpecialtyDto.cs
|   |           SpecialtyUpdateDto.cs
|   |           
|   +---Mappers
|   |       ConsultationMapper.cs
|   |       DoctorMapper.cs
|   |       MedicamentMapper.cs
|   |       OrdonnanceLigneMapper.cs
|   |       OrdonnanceMapper.cs
|   |       PatientMapper.cs
|   |       SpecialtyMapper.cs
|   |       
|   \---Services
|           ConsultationService.cs
|           DoctorService.cs
|           MedicamentService.cs
|           OrdonnanceLigneService.cs
|           OrdonnanceService.cs
|           PatientService.cs
|           SpecialtyService.cs
|           
          
+---Data
|       medicaments.csv
|       specialties.csv
|       
+---Domain
|   \---Entities
|           Consultation.cs
|           Doctor.cs
|           Medicament.cs
|           Ordonnance.cs
|           OrdonnanceLigne.cs
|           Patient.cs
|           Specialty.cs
|           
+---Infrastructure
|   |   HospitalDbContext.cs
|   |   SeedData.cs
|   |   
|   +---Configurations
|   |       .keep
|   |       ConsultationConfiguration.cs
|   |       DoctorConfiguration.cs
|   |       MedicamentConfiguration.cs
|   |       OrdonnanceConfiguration.cs
|   |       OrdonnanceLigneConfiguration.cs
|   |       PatientConfiguration.cs
|   |       SpecialtyConfiguration.cs
|   |       
|   +---Migrations
|   |       .keep
|   |       20251119122428_InitialCreate.cs
|   |       20251119122428_InitialCreate.Designer.cs
|   |       HospitalDbContextModelSnapshot.cs
|   |       
|   \---Repositories
|           .keep
|           ConsultationRepository.cs
|           DoctorRepository.cs
|           MedicamentRepository.cs
|           OrdonnanceLigneRepository.cs
|           OrdonnanceRepository.cs
|           PatientRepository.cs
|           SpecialtyRepository.cs
|           
+---Logs
|       log-20251119.txt
|       log-20251120.txt
|       log-20251121.txt
                
+---Presentation
|   \---Controllers
|           ConsultationsController.cs
|           DoctorsController.cs
|           MedicamentsController.cs
|           OrdonnancesController.cs
|           PatientsController.cs
|           SpecialtiesController.cs
|           
\---Properties
        launchSettings.json
        
    
---

## 📌 Signification des dossiers

### ✔ Application  
Contient :
- Les DTOs utilisés par l’API  
- Les services (use cases / logique métier)  
- Les mappers générés avec Mapperly  

### ✔ Domain  
Contient **uniquement les entités du domaine** (POCO), sans dépendance.

### ✔ Infrastructure  
Contient :
- Le `DbContext` EF Core  
- Les configurations (FK, PK, contraintes…)  
- Les migrations  
- Le seeding  
- Les repositories  

### ✔ Presentation  
Contient **les contrôleurs API**, un par entité ou groupe d'entités.

### ✔ Data  
Contient les fichiers CSV de seed initial (spécialités, médicaments).

---
# 🧬 Modèle de Domaine (UML)

Le diagramme suivant représente les entités du domaine et leurs relations.  
Il correspond exactement au modèle utilisé dans le projet KingdomHospital.

Specialty (1) ──── (∞) Doctor
Doctor (1) ───── (∞) Consultation
Patient (1) ───── (∞) Consultation

Consultation (1) ──── (∞) Ordonnance
Ordonnance (1) ──── (∞) OrdonnanceLigne

Medicament (1) ──── (∞) OrdonnanceLigne


---

# 📘 Détails des Entités

## **Specialty**


Id (PK)
Name (unique, max 30)

Relation :
- 1 spécialité → plusieurs médecins

---

## **Doctor**


Id (PK)
LastName
FirstName
SpecialtyId (FK → Specialty)

Contraintes métier :
- SpecialtyId doit exister  
- Pas de doublon exact (Nom + Prénom + Spécialité)

Relations :
- 1 docteur → plusieurs consultations  
- 1 docteur → plusieurs ordonnances  

---

## **Patient**


Id (PK)
LastName
FirstName
BirthDate

Contraintes :
- BirthDate réaliste (>= 1900, <= aujourd’hui)
- Unicité logique : Nom + Prénom + BirthDate

Relations :
- 1 patient → plusieurs consultations  
- 1 patient → plusieurs ordonnances  
- Impossible de supprimer un patient ayant un historique

---

## **Consultation**


Id (PK)
DoctorId (FK)
PatientId (FK)
Date (DateOnly)
Hour (TimeOnly)
Reason

Contraintes importantes :
- Un médecin **ne peut pas** avoir deux consultations à la même date + heure  
- Un patient **ne peut pas** avoir deux consultations à la même date + heure  
- DoctorId et PatientId doivent exister

Relation :
- 1 consultation → 0 ou plusieurs ordonnances

---

## **Ordonnance**


Id (PK)
DoctorId (FK)
PatientId (FK)
ConsultationId (FK nullable)
Date
Notes (max 255)


Contraintes :
- Si liée à une consultation → même docteur + même patient  
- Date ordonnance >= date consultation  
- Impossible de supprimer si lignes présentes (mais cascade gérée dans le service)

Relations :
- 1 ordonnance → 1 à N lignes  
- 1 médicament peut apparaître dans plusieurs ordonnances

---

## **OrdonnanceLigne**


Id (PK)
OrdonnanceId (FK)
MedicamentId (FK)
Dosage (<=50)
Frequency (<=50)
Duration (<=30)
Quantity (>0)
Instructions (<=255, optionnel)


Contraintes :
- Quantity > 0  
- Champ obligatoire : Dosage, Frequency, Duration  
- Pas de ligne strictement dupliquée dans la même ordonnance  
  (Même médicament + même dosage + durée + fréquence)

---

## **Medicament**


Id (PK)
Name (unique, max 100)
DosageForm (max 30)
Strength (max 30)
AtcCode (max 20)


Relation :
- 1 médicament → plusieurs lignes d’ordonnance

---

# 🧩 Résumé des relations

| Entité A        | Relation | Entité B           |
|-----------------|----------|--------------------|
| Specialty       | 1 ─ ∞    | Doctor             |
| Doctor          | 1 ─ ∞    | Consultation       |
| Patient         | 1 ─ ∞    | Consultation       |
| Consultation    | 1 ─ ∞    | Ordonnance         |
| Ordonnance      | 1 ─ ∞    | OrdonnanceLigne    |
| Medicament      | 1 ─ ∞    | OrdonnanceLigne    |

---

# 🔗 Contraintes clés 

- **Double-booking interdit** (même médecin ou même patient à même horaire)  
- **Cohérence consultation / ordonnance** (doctorId & patientId doivent correspondre)  
- **ConsultationId nullable** pour permettre de détacher une ordonnance  
- **Patient / Doctor non supprimables** s’ils ont un historique  
- **Spécialité non supprimable** si des médecins y sont rattachés  
- **Médicament non supprimable** si utilisé dans des ordonnances  
- **Unicités** :
  - Specialty.Name  
  - Medicament.Name  
  - Patient (Nom + Prénom + BirthDate)  

---
