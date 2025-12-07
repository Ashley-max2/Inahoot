using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ErrorReport
{
    public string errorType;
    public string errorMessage;
    public string stackTrace;
    public string dateTime;
    public string fileName;
    public string additionalInfo;
    
    public ErrorReport(string type, string message, string trace, string info = "")
    {
        errorType = type;
        errorMessage = message;
        stackTrace = trace;
        dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        fileName = $"ErrorReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        additionalInfo = info;
    }
}

public class ErrorReportManager : MonoBehaviour
{
    public static ErrorReportManager Instance { get; private set; }
    
    private string errorReportsPath;
    private List<ErrorReport> errorReports = new List<ErrorReport>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Usar DataPathManager para rutas visibles
        errorReportsPath = DataPathManager.GetErrorReportsPath() + "/";
        
        if (!Directory.Exists(errorReportsPath))
        {
            Directory.CreateDirectory(errorReportsPath);
        }
        
        LoadExistingReports();
    }

    /// <summary>
    /// Registra un error relacionado con JSON corrupto o mal formateado
    /// </summary>
    public void ReportJSONError(string fileName, string errorMessage, Exception ex = null)
    {
        string message = $"ERROR DE LECTURA JSON\n" +
                        $"Archivo: {fileName}\n" +
                        $"Descripción: {errorMessage}\n" +
                        $"Causa probable: Formato JSON incorrecto o archivo corrupto";
        
        string trace = ex != null ? ex.StackTrace : "No hay stack trace disponible";
        string additionalInfo = $"Ruta del archivo: {fileName}\n" +
                               $"Tipo de excepción: {(ex != null ? ex.GetType().Name : "N/A")}";
        
        ErrorReport report = new ErrorReport("JSON_ERROR", message, trace, additionalInfo);
        SaveErrorReport(report);
        
        
    }

    /// <summary>
    /// Registra un error relacionado con XML corrupto o no encontrado
    /// </summary>
    public void ReportXMLError(string fileName, string errorMessage, Exception ex = null)
    {
        string message = $"ERROR DE LECTURA XML\n" +
                        $"Archivo: {fileName}\n" +
                        $"Descripción: {errorMessage}\n" +
                        $"Causa probable: Formato XML incorrecto, archivo corrupto o no encontrado";
        
        string trace = ex != null ? ex.StackTrace : "No hay stack trace disponible";
        string additionalInfo = $"Ruta del archivo: {fileName}\n" +
                               $"Tipo de excepción: {(ex != null ? ex.GetType().Name : "N/A")}\n" +
                               $"Nota: El sistema mostrará un leaderboard vacío como fallback";
        
        ErrorReport report = new ErrorReport("XML_ERROR", message, trace, additionalInfo);
        SaveErrorReport(report);
        
        
    }

    /// <summary>
    /// Registra un error genérico del sistema
    /// </summary>
    public void ReportGeneralError(string errorType, string errorMessage, Exception ex = null)
    {
        string message = $"ERROR DEL SISTEMA\n" +
                        $"Tipo: {errorType}\n" +
                        $"Descripción: {errorMessage}";
        
        string trace = ex != null ? ex.StackTrace : "No hay stack trace disponible";
        string additionalInfo = ex != null ? $"Excepción: {ex.Message}" : "";
        
        ErrorReport report = new ErrorReport(errorType, message, trace, additionalInfo);
        SaveErrorReport(report);
        
        
    }

    private void SaveErrorReport(ErrorReport report)
    {
        errorReports.Add(report);
        
        try
        {
            string fullPath = Path.Combine(errorReportsPath, report.fileName);
            
            string reportContent = GenerateReportContent(report);
            
            File.WriteAllText(fullPath, reportContent);
            
            
        }
        catch (Exception ex)
        {
            
        }
    }

    private string GenerateReportContent(ErrorReport report)
    {
        return $"═══════════════════════════════════════════════════════\n" +
               $"           INFORME DE ERROR - INAHOOT\n" +
               $"═══════════════════════════════════════════════════════\n\n" +
               $"Fecha y Hora: {report.dateTime}\n" +
               $"Tipo de Error: {report.errorType}\n\n" +
               $"DESCRIPCIÓN DEL ERROR:\n" +
               $"{report.errorMessage}\n\n" +
               $"INFORMACIÓN ADICIONAL:\n" +
               $"{report.additionalInfo}\n\n" +
               $"STACK TRACE:\n" +
               $"{report.stackTrace}\n\n" +
               $"═══════════════════════════════════════════════════════\n" +
               $"Este informe ha sido generado automáticamente.\n" +
               $"Ubicación: {DataPathManager.GetErrorReportsPath()}/\n" +
               $"═══════════════════════════════════════════════════════\n";
    }

    private void LoadExistingReports()
    {
        try
        {
            string[] files = Directory.GetFiles(errorReportsPath, "ErrorReport_*.txt");
            
        }
        catch (Exception ex)
        {
            
        }
    }

    public List<string> GetErrorReportsList()
    {
        List<string> reports = new List<string>();
        
        try
        {
            string[] files = Directory.GetFiles(errorReportsPath, "ErrorReport_*.txt");
            
            foreach (string file in files)
            {
                reports.Add(Path.GetFileName(file));
            }
            
            reports.Sort();
            reports.Reverse(); // Más recientes primero
        }
        catch (Exception ex)
        {
            
        }
        
        return reports;
    }

    public string ReadErrorReport(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(errorReportsPath, fileName);
            
            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath);
            }
            else
            {
                return "Error: Informe no encontrado";
            }
        }
        catch (Exception ex)
        {
            return $"Error al leer informe: {ex.Message}";
        }
    }

    public void DeleteErrorReport(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(errorReportsPath, fileName);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    public void ClearAllReports()
    {
        try
        {
            string[] files = Directory.GetFiles(errorReportsPath, "ErrorReport_*.txt");
            
            foreach (string file in files)
            {
                File.Delete(file);
            }
            
            errorReports.Clear();
            
        }
        catch (Exception ex)
        {
            
        }
    }
}
// Unity recompilation trigger

