using System.ComponentModel;

namespace Skills.Models.Enums;

public enum ImportType
{
    [Description("Supprime toutes les compétences, types et soft-skills puis importe le fichier.")]
    Purge,
    
    [Description("Importe le fichier en ajoutant les lignes inexistantes en base seulement.")]
    AddOnly,
    
    [Description("Remplace les compétences (mappings) présentes dans le fichier importé et en créé de nouvelles. Celles-ci ne seront pas attribuées de nouveau aux collaborateurs !")]
    Replace
}