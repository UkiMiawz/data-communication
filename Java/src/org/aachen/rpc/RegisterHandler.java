package org.aachen.rpc;

import java.util.HashMap;
import org.aachen.rpc.Bully;

public class RegisterHandler {
	public String joinNetwork(String ipAddress) {
		int machinesNumber = JavaWsServer.addMachineToMap(ipAddress);
        return "A new computer join the network : I come in peace " + ipAddress + ". Number of machines now " + machinesNumber;
	}
	
	public String leaderElection() {
		//get machines
		HashMap<Integer, String> machines = JavaWsServer.getMachines();
		Bully bullyGenerator = new Bully(machines);
		bullyGenerator.holdElection(1);
		Integer keyMaster = bullyGenerator.getMaster();
		JavaWsServer.setMaster(keyMaster);
		return machines.get(keyMaster);
	}
}