using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ResourceHandler
{
    private static String classNameLog = "ResourceHandler : ";

    public String getString(String ipAddress)
    {
        Console.WriteLine(classNameLog + "Read resource request from " + ipAddress);
        return CSharpRpcServer.getSharedString();
    }

    public String setString(String newString, String ipAddress)
    {
        Console.WriteLine(classNameLog + "Write resource request from " + ipAddress + " with new string " + newString);
        return CSharpRpcServer.setSharedString(newString);
    }

    public String generateRandomString()
    {
        Console.WriteLine(classNameLog + "Generate random string");
        char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        StringBuilder sb = new StringBuilder();
        Random random = new Random();
        for (int i = 0; i < 10; i++)
        {
            char c = chars[random.Next(chars.Length)];
            sb.Append(c);
        }
        String output = sb.ToString();
        Console.WriteLine(classNameLog + "Random string generated => " + output);
        return output;
    }

    public String appendNewString(String myIp, String masterIp, String myString)
    {
        Console.WriteLine(classNameLog + "Append string " + myString);
        String currentString = "";
        String finalString = "";

        //get current string from master

        if (masterIp != myIp)
        {
            Object[] parameters = new Object[] { myIp };
            currentString = (String)XmlRpcHelper.SendToOneMachine(masterIp, GlobalMethodName.resourceGetString, parameters);
        }
        else
        {
            currentString = getString(myIp);
        }

        Console.WriteLine(classNameLog + "Current string " + currentString);

        if (masterIp != myIp)
        {
            Object[] parameters = new Object[] { currentString + myString, myIp };
            finalString = (String)XmlRpcHelper.SendToOneMachine(masterIp, "Resource.setString", parameters);
            Console.WriteLine(classNameLog + "String appended in master, shared string value now " + finalString);
        }
        else
        {
            //generate random string of 10 chars and append
            finalString = setString(currentString + myString, myIp);
            Console.WriteLine(classNameLog + "String appended to me as master, shared string value now " + finalString);
        }

        return finalString;
    }

    public String readNewString(String myIp, String masterIp)
    {
        String currentString = "";
        Console.WriteLine(classNameLog + "Read shared string");

        if (masterIp != myIp)
        {
            //read string from master
            Console.WriteLine(classNameLog + "Read shared string from master => " + masterIp);
            Object[] parameters = new Object[] { myIp };
            currentString = (String)XmlRpcHelper.SendToOneMachine(masterIp, GlobalMethodName.resourceGetString, parameters);
        }
        else
        {
            //read from myself
            Console.WriteLine(classNameLog + "Read shared string from me as master => " + masterIp);
            currentString = getString(myIp);
        }

        Console.WriteLine(classNameLog + "String read, shared string value now " + currentString);
        return currentString;
    }
}

