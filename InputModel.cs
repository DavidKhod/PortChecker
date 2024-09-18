using System.Net;

namespace PortSniffer
{
    public class InputModel
    {
        private List<string>? _addressList;
        private string _input;
        private string? _flag = null;
        private string? _outputFlag = null;
        public void Input()
        {
            Console.Write("Enter a domain or an ip address: ");
            _input = Console.ReadLine();
            while (string.IsNullOrEmpty(_input))
            {
                Console.Write("Please Enter a valid domain or an ip address: ");
                _input = Console.ReadLine();
            }
            SplitInputIntoFlags();
        }

        public string[] GetIPList()
        {
            if (string.IsNullOrEmpty(_input))
            {
                throw new Exception("Must initiate input before calling GetIPList() method, Input() method hasn't been called");
            }
            if (IsDomain(_input))
            {
                try
                {
                    List<IPAddress> IPAddressList = Dns.GetHostAddresses(_input).ToList();
                    if (IPAddressList.Count == 1 && IsValidIP(IPAddressList[0].ToString()))
                    {
                        return new string[] { IPAddressList[0].ToString() };
                    }
                    else
                    {
                        ConvertToValidIPList(IPAddressList);
                        if (_addressList.Count == 1)
                        {
                            return _addressList.ToArray();
                        }
                        Console.WriteLine($"List of ip addresses:");
                        int cnt = 1;
                        foreach (string IPAddress in _addressList)
                        {
                            Console.WriteLine($"{cnt}: {IPAddress}");
                            cnt++;
                        }
                        Console.WriteLine();
                        return AcceptChoice();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to resolve domain, {ex.ToString()}");
                }
            }
            else
            {
                if (IsValidIP(_input))
                {
                    return new string[] { _input };
                }
            }
            throw new Exception("IP is not valid");
        }

        public void InitByFlags(ref PortSnifferModel portSnifferModel, ref OutPutModel outPutModel)
        {
            if (string.IsNullOrEmpty(_flag) && string.IsNullOrEmpty(_outputFlag))
                return;
            else
            {
                if (!string.IsNullOrEmpty(_flag))
                {
                    int[] tempPorts = { };//temp value
                    switch (_flag)
                    {
                        case "-a":
                            tempPorts = new int[1000];
                            for (int i = 0; i < tempPorts.Length; i++)
                            {
                                tempPorts[i] = i + 1;
                            }
                            break;
                            //implement more options

                    }//Possibly implement statments to change the connection type? like - f or --help
                    portSnifferModel.ChangePorts(tempPorts);
                }
                if (!string.IsNullOrEmpty(_outputFlag))
                {
                    outPutModel.SetFlag(_outputFlag);
                }
            }
        }

        private void SplitInputIntoFlags()
        {
            if (_input.Split(" ").Length > 1)
            {
                _flag = _input.Split(" ")[1];
                if (_input.Split(" ").Length == 3)
                {
                    _outputFlag = _input.Split(" ")[2];
                }
            }
            _input = _input.Split(" ")[0];
            switch (_flag)//Making a place for future implemintations
            {
                case "-wf":
                    _outputFlag = _flag;
                    _flag = null;
                    break;
            }
        }

        private bool IsValidIP(string? ip)//Checks if an IP is a valid IPV4
        {
            if (string.IsNullOrEmpty(ip))
                return false;
            string[] octets = ip.Split(".");
            if (octets.Length != 4)
                return false;
            foreach (string digit in octets)
            {
                int number = int.Parse(digit);
                if (number < 0 && number > 255)
                    return false;
            }
            return true;
        }

        private bool IsDomain(string input)
        {
            bool isDomain = false;
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    isDomain = true;
                    break;
                }
            }
            return isDomain;
        }

        private string[] AcceptChoice()
        {
            Console.Write($"Choose an ip by id, or press A for all: ");
            string? choice = Console.ReadLine();
            if (choice.ToUpper().Equals("A"))
            {
                return _addressList.ToArray();
            }
            try
            {
                int.TryParse(choice, out int id);
                if (id < 0 || id > _addressList.Count)
                {
                    throw new Exception("Number not in the list");
                }
                return new string[] { _addressList[id - 1] };
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid input, must be a number in the list or the letter 'A' " + ex);
            }
        }

        private void ConvertToValidIPList(List<IPAddress> IPAddressList)
        {
            _addressList = new List<string>();
            foreach (var IPAddress in IPAddressList)
            {
                if (IsValidIP(IPAddress.ToString()))
                {
                    _addressList.Add(IPAddress.ToString());
                }
            }
        }

    }
}
