using System.ComponentModel;

namespace Skills.Models.Enums;

public enum ImportState
{
    [Description("Tâche annulée à cause d'un incident.")]
    Cancelled,
    
    [Description("Tâche complétée sans problème.")]
    Successful,
    
    [Description("Le traitement a planté à cause d'une erreur.")]
    Crashed,
    
     [Description("Le traitement n'a pas été effectué pour une raison inconnue.")]
    Skipped
}