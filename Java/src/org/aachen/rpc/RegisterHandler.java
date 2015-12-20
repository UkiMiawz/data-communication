package org.aachen.rpc;

public class RegisterHandler {
	public String joinNetwork(String ipAddress) {
		int machinesNumber = JavaWsServer.addMachineToMap(ipAddress);
        return "A new computer join the network : I come in peace " + ipAddress + ".Number of machines now " + machinesNumber;
	}
}