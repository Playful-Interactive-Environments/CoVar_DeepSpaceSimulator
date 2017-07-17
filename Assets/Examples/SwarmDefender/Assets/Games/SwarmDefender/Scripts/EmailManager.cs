using ImapX;
using SwarmDefender;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;


/// <summary>
/// This class connects to two separate IMAP email accounts and periodically checks them for new messages. Once a new message is received, it is parsed and interactibles are generated.
/// </summary>
public class EmailManager : MonoBehaviour
{
    // The client instance for checking the obstacle emails
    private ImapClient client;

    // Connection data for the imapObstacle accounts
    private string host = "imap.gmail.com";
    private int port = 993;
    private bool ssl = true;

    // Login data for the IMAP accounts
    private string usernameSwarm = "swarmdefender@gmail.com";
    private string password = "Hagenberg.2015";

    // The target active folder to be worked with
    private Folder activeFolder;

    // The archive folder where processed messages go to
    private Folder archiveFolder;
    const string ARCHIVE_FOLDER_NAME = "Processed";

    // Holds the status of the email loop
    private bool isCheckingForEmail;

    // Defines the email polling interval
    private const int timeBetweenEmailChecks = 10000;

    // Holds the timestamp of the last email check
    private int lastEmailCheck;
    // The regular expression to filter out the name and intention of the audience
    private const string emailPattern = @"[0-9A-Za-z_ !:;&+'öäüßÖÄÜ-]{1,140}";

    private Queue<string> invaderNames;

    // Use this for initialization
    void Start()
    {
        isCheckingForEmail = false;
        InitializeEmailChecking();
        invaderNames = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.LogWarning("Trying to reconnect to the email server...");
            InitializeEmailChecking();
        }

        lock (invaderNames)
        {
            while (invaderNames.Count > 0)
            {
                //InteractiblesManager.GetInstance().EnqueueObstacle(swarmNames.Dequeue());
                InvaderManager.Instance.EnqueueInvader(invaderNames.Dequeue());
            }
        }
    }

    void OnDestroy()
    {
        isCheckingForEmail = false;
        client.Logout();
        client.Disconnect();
        Debug.LogWarning("Shutting down email checking.");
    }

    private void InitializeEmailChecking()
    {
        if (!isCheckingForEmail)
        {
            // Create a new client
            client = new ImapClient(host, ssl, false);
            // See if we can connect to the host
            if (client.Connect())
            {
                // If we're connected, log in
                if (client.Login(usernameSwarm, password))
                {
                    // Choose the active folder where mails are processed and the archived where they are moved to
                    activeFolder = client.Folders.Inbox;
                    archiveFolder = client.Folders[ARCHIVE_FOLDER_NAME];

                    // Tell the client to auto-download messages in the folders
                    client.Behavior.AutoPopulateFolderMessages = true;

                    // Start the thread for continuos email obstacleChecking
                    isCheckingForEmail = true;
                    lastEmailCheck = Environment.TickCount;
                    Thread emailThread = new Thread(new ThreadStart(CheckForNewInvaderNameMessages));
                    emailThread.Name = "Email Invader Names";
                    emailThread.IsBackground = true;
                    emailThread.Start();
                    Debug.Log("Connection to email account established. Beginning to check for e-mails with invader names.");
                }
                else
                {
                    Debug.LogWarning("Login to the e-mail account did not succeed!");
                }
            }
            else
            {
                Debug.LogWarning("Connection to the e-mail server has failed!");
            }
        }

    }

    /// <summary>
    /// Constantly polls the email server every 10 seconds or so for new messages.
    /// </summary>
    private void CheckForNewInvaderNameMessages()
    {
        while (isCheckingForEmail)
        {
            if (Environment.TickCount - lastEmailCheck > timeBetweenEmailChecks)
            {
                Debug.Log("Time for email checking!");
                // Select the active folder every time so it's accessible
                activeFolder.Select();
                // Get all unseen messages ("ALL" should work as well)
                Message[] messages = activeFolder.Search("UNSEEN");
                Debug.Log("Number of new messages: " + messages.Length);
                foreach (Message message in messages)
                {
                    // Process every messages, mark it as read and move it to the archive
                    ParseMessage(message);
                    message.Seen = true;
                    message.MoveTo(archiveFolder);
                }
                lastEmailCheck = Environment.TickCount;
            }
        }
        Debug.Log("E-mail thread has run out.");
    }

    private void ParseMessage(Message message)
    {
        string content = "Invader";

        // If there's text in the subject line (check for null or blank - depends on the client)
        if (message.Subject != null && !message.Subject.Trim().Equals(String.Empty))
        {
            content = message.Subject.Trim();
            Debug.Log("Subject: " + content);
        }
        // if it's an email with blank subject line go for the body...
        else
        {
            // Make sure that the text in the body is not null since we're calling trim
            if (message.Body.Text != null)
            {
                content = message.Body.Text.Trim();
            }
            // If it was null (for what ever reason) we set it as a blank string (will be denied by the regex check later on)
            else
            {
                content = "";
            }
            Debug.Log("Body: " + content);
        }

        // The regex check filters out unwanted characters and makes sure the names consist only of displayable characters
        Regex regex = new Regex(emailPattern, RegexOptions.IgnoreCase);

        Match match = regex.Match(content);
        if (match.Success)
        {
            // If the regex worked, enqueue the stripped name to our InvaderManager
            string name = match.Groups[0].Value.Trim();

            lock (invaderNames)
            {
                invaderNames.Enqueue(name);
            }
        }
        else
        {
            // If not, just display a debug warning (we silently ignore this in the game - tough luck)
            Debug.LogWarning("Wrong message format. Ignoring: " + content);
        }
    }
}