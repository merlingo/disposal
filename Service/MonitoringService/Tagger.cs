using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{

    public class Tagger : Filter 
    {
        public static List<string> tags = new List<string>() {"SQL Injection","SSI Injection","SSJS Injection","Injection","ASP-JS Injection","ASP-VBS Injection","PHP Injection","Java Injection", "Python Injection","Perl Injection","Ruby Injection","PHP Object Injection","PHP preg_replace Abuse",
        "ABAP Injection","OS Command Injection","Format String Injection","EL Injection","RoR YAML Injection", "EL3 Injection","Memcached Injection","HQL Injection","ORM Injection","Mongo NoSQL Injection","LDAP Injection",
"Escape Sequence Injection","HTTP Request Injection","Reflection Injection","XQUERY Injection","XPATH Injection","CSPP","ADS","OGNL Expression Injection","Unsigned Server Control Property Injection","EoDSeC",
        "SQL Filter Injection","SQL Rowset Injection","Null-Byte Injection","Poison Null Byte","Embedding Null Code","SMTP Injection","IMAP Injection","POP3 Injection","Email Header Injection","HTTP Request Header Injection",
        "XML Injection","Special Element Injection","XCS","XSS","Cross-Site Scripting","Cross Site Scripting","DOM XSS","Flash XSS","FPI","Flash Parameter Injection","Flash Injection","XSF","RFD","Malicious File Download","Unvalidated Redirect",
        "HTTP Response Splitting","HTTP Response Smuggling","CRLF","Log Forging","WebView Injection","Unvalidated Forward","Content Spoofing","JSON Hijacking","XSSI","Cross Site Request Forgery","CSRF","Dynamic Ajax CSRF","SDRF",
            "Same Domain Request Forgery","Clickjacking","CSWSH","Cross Site WebSocket Hijacking","Frame Spoofing","PHP Remote File Inclusion","Malicious File Execution","JSP Remote File Inclusion","Client-Controlled Price Manipulation",
"Client-Controlled User Identifier Manipulation","Client-Controlled Privilege Manipulation","Remote XSL Inclusion","Perl Remote File Inclusion","Path Traversal","Path Manipulation","PHP Local File Inclusion","JSP Local File Inclusion",
    "XSS via Remote File Inclusion","MVC Mass Assignment","XXE","XML External Entity Processing", "Server Control Signed Property Override","Insecure Direct Object Reference",
    "Client-Controlled Sum Abuse","Client-Controlled Quantity Manipulation","Client-Controlled Authentication Status Manipulation","Client-Controlled Multiphase Process State Flags Manipulation","Setting Manipulation","Authentication Bypass via Alternative IP Access",
"Authentication Bypass","Generic Business Logic Attack","Generic Session Poisoning Attack","Perl Local File Inclusion","ABAP Process Control","Dynamic Calls","Process Control","Execution of Unsigned Dormant Server Controls",
"Execution of Signed Dormant Server Controls via Cache Reuse","Recovery Destination Manipulation via Parameter Tampering","Client Controlled Action Type Manipulation via Parameter Tampering","XML Signature - Key Retrieval XSA",
"XML Routing Detour","Reverse Proxy Bypass","SSRF","Server Side Request Forgery","HTTP Request Smuggling","Client-Controlled Lock Counter Manipulation","Client-Controlled Lock Flag Manipulation","XML Schema Poisoning",
"Generic Session Puzzling Attack","Authentication Bypass via Forced Browsing","Privilege Abuse","Multiphase Process Bypass via Forced Browsing","Flow Bypass","Forced Browsing","Authentication Bypass via HTTP Verb Tampering","Authorization Bypass via HTTP Verb Tampering",
"Authentication Bypass via Session Puzzling","User Impersonation via Session Puzzling","Privilege Elevation via Session Puzzling","Multiphase Process Bypass via Session Puzzling","Password Recovery Destination Manipulation via Session Puzzling",
"Execution of Unvalidated Dormant Server Controls","EodSec","Unauthorized Administrative Interface Access","Execution After Redirect","SOAPAction Spoofing","Unauthorized WebSocket Access","Source Code Disclosure via Accessible Folder",
"Enumeration of Obsolete and Unreferenced Files","Predictable Resource Location Enumeration","Secret Argument Modification","HTTP MKCOL Method Abuse","Secret Parameter","Application Backdoor","HTTP MKCOL Method Abuse","SQL Execution",
"Malicious File Upload","Remote Binary Planting","HTTP PUT Attack","Information Disclosure","PHP Uploaded File Variables Abuse","SQL Sorting","Generic User Account Privilege Abuse","HTTP DELETE Attack",
"HTTP COPY Method Abuse","HTTP MOVE Method Abuse","User Impersonation via Social Login Design Flaw","Subdomain Takeover via Abuse of Subdomain Claims","Unrestricted File Upload","Account Lockout Abuse",
"Session Fixation","HTTP PROPPATCH Method Abuse","HTTP MKDIR Method Abuse","HTTP PROPFIND Method Abuse","HTTP SEARCH Method Abuse","Logging of Excessive Data","XST","Proxy Abuse","HTTP LOCK Method Abuse","Buffer Overflow",
"Stack Overflow","Heap Overflow","Overflow Variables and Tags","Use After Free","SOAP Array Overflow","User Controlled Memory Pointer Reference","Double Free","Memory Leak","Null Dereference","Expired Pointer Dereference",
"Buffer Underwrite","Integer Overflow","POODLE","Padding Oracle On Downgraded Legacy Encryption","Predictable Session Identifier Abuse","Padding Oracle","BEAST","Browser Exploit Against SSL TLS",
"Browser Reconnaissance Exfiltration via Adaptive Compression of Hypertext","BREACH","HPP","HTTP Parameter Pollution","CAPTCHA Re-Riding","Client-side CAPTCHA Logic Abuse","Chosen CAPTCHA Text Abuse","Arithmetic CAPTCHA Abuse",
"CAPTCHA Rainbow Tables","CAPTCHA Fixation","In-Session CAPTCHA Brute-forcing","OCR-assisted CAPTCHA Brute-forcing","Limited CAPTCHA Repository Abuse","CAPTCHA Clipping","Missing Account Lockout Abuse","Session Stored Lockout Flags Abuse",
"Insecure Password Recovery Initiation Destination","Weak Recovery Answer Enumeration","SSL Renegotiation","SSL Version Rollback","Cipher Suite Rollback","CRIME","Compression Ratio Info-leak Made Easy","TIME",
"Timing Info-leak Made Easy","RC4 Attack","RC4 TLS Attack","Lucky 13","SSL CCS MITM","CCS Injection","Weak Cipher Brute Forcing","Weak SSL Key-Pair Brute Forcing","Insecure Transport Layer Protection","Insecure SSL Protocol Support","Weak Initial Password Generation",
"Weak Recovery Question Selection","Predictable Password Recovery Token Enumeration","Predictable Anti-CSRF Token Abuse","Anti-CSRF Verification Bypass","XML Signature Wrapping","Hash Length Extension","Weak Lockout Policy Abuse",
"Insufficient Logging Abuse","Log Repudiation Attack","Inadequate Storage Encryption Key Strength","Insecure Storage Cryptographic Algorithm","Insecure Credential Hashing Algorithm","Unsalted Hash","Hashing Algorithm", "Missing Required Cryptographic Step",
"Ineffective Session Termination","Unrestricted Recovery Initiation","Persistent Password Recovery Token","Incomplete Session Termination in SSO","Persistent Session Lifespan","Insufficient Logout Visibility",
"Insufficient Session Expiration","TOCTTOU Transaction Race Condition","TOCTTOU","Context Switching Race Condition","Race Condition","Member Field Race Condition","Temporal Session Race Conditions","Single Handler Race Condition",
"Switch-Case Race Condition","Alternate Channel Race Condition","Permission Race Condition During Resource Copy","Link Following Race Condition","Generic Race Condition within a Thread","Cross-Domain Search Timing","Pixel Perfect Timing Attacks",
"Username Enumeration in Login","Username Enumeration in Password Recovery","Username Enumeration in Registration","Username Enumeration","Generic Username Enumeration","Password Brute Forcing","Weak Password Policy",
"Remote Timing Attack","Dir and File Brute Forcing","Forced Deadlock","Web Server Thread Occupation","HTTP Fragmentation Attack","THC-SSL-DoS","XML Bomb","Regular Expression DoS","Database Connection Pool Consumption",
"Floating Point DoS","Hash Collision DoS","Resource Exhaustion","SOAP Coercive Parsing","XML Transformation DOS","XML Signature - Key Retrieval DOS","Over-sized XML DoS","XML Reference Redirect DoS","SOAP Recursive Cryptography DoS",
"Referral Flood of Trusted Entities","HTTP Flood","Credentials Eavesdropping from Unencrypted Channel","Session Hijacking","Unencrypted Communication Eavesdropping","SSL Stripping","Session Replay","MITM",
"Man-In-The-Middle","Surf Jacking","Directory Listing","Password Disclosure in Password Recovery","Generic Password Disclosure","WSDL Disclosure","XML Entity Reference Attack","Sensitive Information Disclosure in Log Files",
"Missing Encryption of Sensitive Data","Hard-coded Cryptographic Key","Hard-coded Credentials","Intent Intercept","Intent Spoof","IIS Short File Name Enumeration","CORS Functionality Abuse","Authentication Bypass via Referer Spoofing",
"Authentication Bypass via IP Spoofing","DNS Rebinding","Invalid SSL Certificate","Expired SSL Certificate","Stolen Expired Certificate Abuse","Stolen Revoked Certificate Abuse","Valid Certificate Abuse for Another Domain",
"Broken Chain-of-Trust Certificate Abuse","Endpoint Impersonation in an Encrypted Communication Channel","Search Engine Impersonation","User Agent Impersonation","ShellShock","HeartBleed","Winshock","UDDI Spoofing"
        };
        string objectName;
        public Tagger( string objectName)
        {
            //https://cve.circl.lu/api/cve/:vid adresine istek gönderilerek response json'dan vulnerable_product çekilmesi ve : ile seperate edilince 0dan başlayınca 3. vendor 4.product 5.version
            this.objectName = objectName;
        }
        public override Chunk filter(Chunk datasource, EventBus eb)
        {
            //vulnerability içinden desc ile tag listesindekini eslestirip eslesenleri gndermek
            VulnerabilityDataModel dm = (VulnerabilityDataModel) datasource.dm;
            for (int i = 0; i < tags.Count; i++)
            {
                if (datasource.rawString.Contains(tags[i]))
                {
                    string tag = tags[i];
                    string json = "{\"cveid\":\"" + dm.cve + "\",\"name\":\"" + tag + "\"}";
                    List<MatcherDataModel> tagmodels = (List<MatcherDataModel>)DataModel.isExistInRepo<TagDataModel>("http://localhost:5000/main/match/m", "cveid=" + dm.cve + "&name=" + tag);
                    if (tagmodels == null)
                    {
                        Event e = new Event(json, DateTime.Now, EventBus.GetLocalIPAddress(), objectName, "Elaborator", "main/tag/m");
                        eb.eventBasedTrigger(e);
                    }
                }
            }
            

            return datasource;
        }
    }
}
