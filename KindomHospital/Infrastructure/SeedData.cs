using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingdomHospital.Domain.Entities;

namespace KingdomHospital.Infrastructure;

public static class SeedData
{
    public static void Initialize(HospitalDbContext context)
    {
        SeedSpecialties(context);
        SeedMedicaments(context);
        SeedDemoData(context);
    }

    private static void SeedSpecialties(HospitalDbContext context)
    {
        // Si déjà des spécialités, on ne refait pas
        if (context.Specialties.Any())
            return;

        var basePath = AppContext.BaseDirectory;
        var filePath = Path.Combine(basePath, "Data", "specialties.csv");

        if (!File.Exists(filePath))
            return;

        var lines = File.ReadAllLines(filePath)
            .Skip(1); // sauter la ligne d’en-tête

        var specialties = new List<Specialty>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');
            if (parts.Length < 2)
                continue;

            var name = parts[1].Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;

            // respecter MaxLength 30 (checklist / modèle) 
            if (name.Length > 30)
                name = name[..30];

            // éviter les doublons
            if (!specialties.Any(s => s.Name == name) &&
                !context.Specialties.Any(s => s.Name == name))
            {
                specialties.Add(new Specialty
                {
                    Name = name
                });
            }
        }

        if (specialties.Count > 0)
        {
            context.Specialties.AddRange(specialties);
            context.SaveChanges();
        }
    }

    private static void SeedMedicaments(HospitalDbContext context)
    {
        if (context.Medicaments.Any())
            return;

        var basePath = AppContext.BaseDirectory;
        var filePath = Path.Combine(basePath, "Data", "medicaments.csv");

        if (!File.Exists(filePath))
            return;

        var lines = File.ReadAllLines(filePath)
            .Skip(1); // header: Id,Name,DosageForm,Strength,AtcCode

        var medicaments = new List<Medicament>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');
            if (parts.Length < 4)
                continue;

            var name = parts[1].Trim();
            var dosageForm = parts[2].Trim();
            var strength = parts[3].Trim();
            var atcCode = parts.Length > 4 ? parts[4].Trim() : null;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(dosageForm) ||
                string.IsNullOrWhiteSpace(strength))
                continue;

            // contraintes de longueur venant de la modélisation Medicament dans l’énoncé
            // Name <= 100, DosageForm <= 30, Strength <= 30, AtcCode <= 20 
            if (name.Length > 100) name = name[..100];
            if (dosageForm.Length > 30) dosageForm = dosageForm[..30];
            if (strength.Length > 30) strength = strength[..30];
            if (!string.IsNullOrEmpty(atcCode) && atcCode.Length > 20)
                atcCode = atcCode[..20];

            // éviter les doublons par Name (Name est censé être unique) 
            if (!medicaments.Any(m => m.Name == name) &&
                !context.Medicaments.Any(m => m.Name == name))
            {
                medicaments.Add(new Medicament
                {
                    Name = name,
                    DosageForm = dosageForm,
                    Strength = strength,
                    AtcCode = atcCode
                });
            }
        }

        if (medicaments.Count > 0)
        {
            context.Medicaments.AddRange(medicaments);
            context.SaveChanges();
        }
    }
                    //chatgpt below
    private static void SeedDemoData(HospitalDbContext context)
    {
        // Si des données existent déjà, on ne refait pas le seed
        if (context.Doctors.Any() ||
            context.Patients.Any() ||
            context.Consultations.Any() ||
            context.Ordonnances.Any())
        {
            return;
        }

        // --- 1) Récupérer quelques spécialités connues (noms venant du CSV / Annexe 1) ---
        var cardio = context.Specialties.FirstOrDefault(s => s.Name == "Cardiologie");
        var derma = context.Specialties.FirstOrDefault(s => s.Name == "Dermatologie");
        var neuro = context.Specialties.FirstOrDefault(s => s.Name == "Neurologie");
        var pedia = context.Specialties.FirstOrDefault(s => s.Name == "Pédiatrie");
        var ortho = context.Specialties.FirstOrDefault(s => s.Name == "Chirurgie Orthopédique");
        var ophtha = context.Specialties.FirstOrDefault(s => s.Name == "Ophtalmologie");

        if (cardio is null || derma is null || neuro is null || pedia is null || ortho is null || ophtha is null)
        {
            // Si les spécialités nécessaires n'existent pas, on ne fait rien (problème de seed CSV)
            return;
        }


        // --- 2) DOCTORS (6 docteurs, plusieurs spécialités) ---
        var doctors = new List<Doctor>
        {
            new() { FirstName = "Gregory", LastName = "House", SpecialtyId = neuro.Id },
            new() { FirstName = "Sarah",   LastName = "Lambert", SpecialtyId = derma.Id },
            new() { FirstName = "James",   LastName = "Wilson",  SpecialtyId = cardio.Id },
            new() { FirstName = "Emma",    LastName = "Martin",  SpecialtyId = pedia.Id },
            new() { FirstName = "Lucas",   LastName = "Morel",   SpecialtyId = ortho.Id },
            new() { FirstName = "Clara",   LastName = "Robert",  SpecialtyId = ophtha.Id },
        };

        context.Doctors.AddRange(doctors);
        context.SaveChanges(); // Ids générés

        // --- 3) PATIENTS (5 patients, 1 sans consultation) ---
        var patients = new List<Patient>
        {
            new() { FirstName = "Alice",  LastName = "Dupont", BirthDate = new DateOnly(1990, 3, 12) },
            new() { FirstName = "Marc",   LastName = "Petit",  BirthDate = new DateOnly(1985, 7, 9) },
            new() { FirstName = "Elise",  LastName = "Martin", BirthDate = new DateOnly(2000, 11, 20) },
            new() { FirstName = "Thomas", LastName = "Leroy",  BirthDate = new DateOnly(1978, 5, 1) },
            // Patient sans consultation 
            new() { FirstName = "Julien", LastName = "Henry",  BirthDate = new DateOnly(1995, 9, 14) }
        };

        context.Patients.AddRange(patients);
        context.SaveChanges();

        // Pour plus de lisibilité
        var d1 = doctors[0]; // House
        var d2 = doctors[1];
        var d3 = doctors[2];
        var d4 = doctors[3];
        var d5 = doctors[4];
        var d6 = doctors[5];

        var p1 = patients[0]; // Alice
        var p2 = patients[1];
        var p3 = patients[2];
        var p4 = patients[3];
        var p5 = patients[4]; // Julien : aucun RDV

        // --- 4) CONSULTATIONS (10, avec contraintes) ---
        // Règles :
        // - 0 à 3 consultations par patient
        // - au moins 1 patient sans consultation (p5)
        // - au moins une journée où un même médecin a deux consultations (d1) à deux heures différentes 

        var baseDate = new DateOnly(2025, 1, 15);

        var consultations = new List<Consultation>
        {
            // Jour 1 : d1 avec 2 consultations (9h, 10h)
            new()
            {
                DoctorId = d1.Id,
                PatientId = p1.Id,
                Date = baseDate,
                Hour = new TimeOnly(9, 0),
                Reason = "Migraines récurrentes"
            },
            new()
            {
                DoctorId = d1.Id,
                PatientId = p2.Id,
                Date = baseDate,
                Hour = new TimeOnly(10, 0),
                Reason = "Douleurs cervicales"
            },

            // Autres consultations réparties
            new()
            {
                DoctorId = d2.Id,
                PatientId = p1.Id,
                Date = baseDate.AddDays(1),
                Hour = new TimeOnly(11, 0),
                Reason = "Éruption cutanée"
            },
            new()
            {
                DoctorId = d3.Id,
                PatientId = p3.Id,
                Date = baseDate.AddDays(1),
                Hour = new TimeOnly(9, 30),
                Reason = "Douleurs thoraciques"
            },
            new()
            {
                DoctorId = d4.Id,
                PatientId = p4.Id,
                Date = baseDate.AddDays(2),
                Hour = new TimeOnly(14, 0),
                Reason = "Suivi pédiatrique"
            },
            new()
            {
                DoctorId = d3.Id,
                PatientId = p1.Id,
                Date = baseDate.AddDays(3),
                Hour = new TimeOnly(10, 30),
                Reason = "Contrôle cardiologique"
            },
            new()
            {
                DoctorId = d5.Id,
                PatientId = p2.Id,
                Date = baseDate.AddDays(3),
                Hour = new TimeOnly(15, 0),
                Reason = "Douleur au genou"
            },
            new()
            {
                DoctorId = d6.Id,
                PatientId = p3.Id,
                Date = baseDate.AddDays(4),
                Hour = new TimeOnly(16, 0),
                Reason = "Baisse de vision"
            },
            new()
            {
                DoctorId = d2.Id,
                PatientId = p4.Id,
                Date = baseDate.AddDays(5),
                Hour = new TimeOnly(9, 0),
                Reason = "Suivi dermatologique"
            },
            new()
            {
                DoctorId = d4.Id,
                PatientId = p1.Id,
                Date = baseDate.AddDays(5),
                Hour = new TimeOnly(10, 0),
                Reason = "Vaccination"
            }
        };

        context.Consultations.AddRange(consultations);
        context.SaveChanges();

        // --- 5) ORDONNANCES (5, avec contraintes) ---
        // Critères :
        // - au moins 5 ordonnances
        // - 1 ordonnance avec 3 médicaments
        // - au moins un patient avec 2 ordonnances (ici p1) 

        var c1 = consultations[0];
        var c2 = consultations[1];
        var c3 = consultations[2];
        var c4 = consultations[3];
        var c5 = consultations[4];

        var ord1 = new Ordonnance
        {
            DoctorId = d1.Id,
            PatientId = p1.Id,
            ConsultationId = c1.Id,
            Date = c1.Date,
            Notes = "Traitement initial"
        };

        var ord2 = new Ordonnance
        {
            DoctorId = d3.Id,
            PatientId = p1.Id,
            ConsultationId = c6Exists(consultations, 5) ? consultations[5].Id : null,
            Date = baseDate.AddDays(3),
            Notes = "Suivi cardiologique"
        };

        var ord3 = new Ordonnance
        {
            DoctorId = d2.Id,
            PatientId = p2.Id,
            ConsultationId = c2.Id,
            Date = c2.Date,
            Notes = "Crème et antihistaminique"
        };

        var ord4 = new Ordonnance
        {
            DoctorId = d4.Id,
            PatientId = p4.Id,
            ConsultationId = c5.Id,
            Date = c5.Date,
            Notes = "Antalgiques légers"
        };

        var ord5 = new Ordonnance
        {
            DoctorId = d3.Id,
            PatientId = p3.Id,
            ConsultationId = c4.Id,
            Date = c4.Date,
            Notes = "Antibiothérapie courte"
        };

        var ordonnances = new List<Ordonnance> { ord1, ord2, ord3, ord4, ord5 };
        context.Ordonnances.AddRange(ordonnances);
        context.SaveChanges();

        // --- 6) LIGNES D’ORDONNANCE ---
        // On récupère quelques médicaments par leur Name (Annexe 2). 
        var paracetamol = context.Medicaments.FirstOrDefault(m => m.Name == "Paracetamol");
        var ibuprofene = context.Medicaments.FirstOrDefault(m => m.Name == "Ibuprofene");
        var amoxicilline = context.Medicaments.FirstOrDefault(m => m.Name == "Amoxicilline");
        var omeprazole = context.Medicaments.FirstOrDefault(m => m.Name == "Omeprazole");
        var salbutamol = context.Medicaments.FirstOrDefault(m => m.Name == "Salbutamol");
        var heparine = context.Medicaments.FirstOrDefault(m => m.Name == "Héparine");

        if (paracetamol is null || ibuprofene is null || amoxicilline is null ||
            omeprazole is null || salbutamol is null || heparine is null)
        {
            // Seed CSV incomplet, on arrête là
            return;
        }

        var lignes = new List<OrdonnanceLigne>
        {
            // Ordonnance 1 : 2 médicaments
            new()
            {
                OrdonnanceId = ord1.Id,
                MedicamentId = paracetamol.Id,
                Dosage = "500mg",
                Frequency = "3x/jour",
                Duration = "5 jours",
                Quantity = 15,
                Instructions = "Après les repas"
            },
            new()
            {
                OrdonnanceId = ord1.Id,
                MedicamentId = ibuprofene.Id,
                Dosage = "400mg",
                Frequency = "2x/jour",
                Duration = "3 jours",
                Quantity = 6,
                Instructions = "Avec un grand verre d'eau"
            },

            // Ordonnance 2 (p1 encore) : 3 médicaments (critère) 
            new()
            {
                OrdonnanceId = ord2.Id,
                MedicamentId = paracetamol.Id,
                Dosage = "500mg",
                Frequency = "3x/jour",
                Duration = "7 jours",
                Quantity = 21,
                Instructions = "Ne pas dépasser la dose"
            },
            new()
            {
                OrdonnanceId = ord2.Id,
                MedicamentId = omeprazole.Id,
                Dosage = "20mg",
                Frequency = "1x/jour",
                Duration = "14 jours",
                Quantity = 14,
                Instructions = "Le matin à jeun"
            },
            new()
            {
                OrdonnanceId = ord2.Id,
                MedicamentId = salbutamol.Id,
                Dosage = "100mcg",
                Frequency = "2 inhalations 3x/jour",
                Duration = "10 jours",
                Quantity = 1,
                Instructions = "Agiter avant usage"
            },

            // Ordonnance 3
            new()
            {
                OrdonnanceId = ord3.Id,
                MedicamentId = ibuprofene.Id,
                Dosage = "400mg",
                Frequency = "2x/jour",
                Duration = "5 jours",
                Quantity = 10,
                Instructions = null
            },

            // Ordonnance 4
            new()
            {
                OrdonnanceId = ord4.Id,
                MedicamentId = paracetamol.Id,
                Dosage = "500mg",
                Frequency = "3x/jour",
                Duration = "3 jours",
                Quantity = 9,
                Instructions = null
            },

            // Ordonnance 5
            new()
            {
                OrdonnanceId = ord5.Id,
                MedicamentId = amoxicilline.Id,
                Dosage = "500mg",
                Frequency = "3x/jour",
                Duration = "7 jours",
                Quantity = 21,
                Instructions = "Finir le traitement même en cas d'amélioration"
            }
        };

        context.OrdonnanceLignes.AddRange(lignes);
        context.SaveChanges();
    }

    // helper pour éviter un out-of-range si je modifie plus tard la liste
    private static bool c6Exists(List<Consultation> consultations, int index)
        => consultations.Count > index;

}
