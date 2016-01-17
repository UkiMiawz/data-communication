package org.aachen.rpc;

import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Objects;
import java.util.TreeMap;

public class Helper {
	
	private static String classNameLog = "Helper : ";
	
	/**
	 * Get key in map by passing value
	 * @param map tree map value
	 * @param value value belong to key to search
	 * @return key object found or null if not found
	 */
	public static <T, E> T getKeyByValue(Map<T, E> map, E value) {
	    for (Entry<T, E> entry : map.entrySet()) {
	        if (Objects.equals(value, entry.getValue())) {
	            return entry.getKey();
	        }
	    }
	    return null;
	}
	
	/**
	 * Convert response from get hash machines to treemap of integer string
	 * @param treeMapResponse object response from Java or C#
	 * @return
	 */
	public static TreeMap<Integer, String> convertMapResponseToMachinesTreeMap(Object treeMapResponse){
		
		System.out.println(classNameLog + "Map response to tree map");
		System.out.println(treeMapResponse);

		TreeMap<Integer, String> hashMapFinal = new TreeMap<Integer, String>();
		boolean isFromJava = false;
		
		HashMap<String, String> hashFromJava = new HashMap<String, String>();
		Object[] objectListFromC = new Object[]{};
		
		if(treeMapResponse instanceof HashMap) {
			System.out.println(classNameLog + "Response is HashMap from JAVA, convert to HashMap of String");
			hashFromJava = (HashMap<String, String>) treeMapResponse;
			isFromJava = true;
		} else {
			System.out.println(classNameLog + "Response is array of object from C#, convert to array of object");
			isFromJava = false;
			objectListFromC = (Object[]) treeMapResponse;
		}
		
		//if containing "IpAddress" and "NetworkPriority" then from C#
		if(!isFromJava && Arrays.deepToString(objectListFromC).contains("IpAddress") && Arrays.deepToString(objectListFromC).contains("NetworkPriority")){
			System.out.println(classNameLog + "Value passed from C#");
			int priority = 0;
			String ipAddress = "";
			for(Object listValue: objectListFromC){
				HashMap<String, String> rawMapValue = (HashMap<String, String>) listValue;
				for(Map.Entry<String,String> entry : rawMapValue.entrySet()) {
					String key = entry.getKey().toString();
					String value = String.valueOf(entry.getValue());
					System.out.println(classNameLog + "Key => " + key + " Value =>" + value);
					
					//detect whether the current hashmap entry is for ip address or network priority
					if(key.contains("NetworkPriority")){
						priority = Integer.parseInt(value);
					} else {
						ipAddress = value;
					}
					
				    if(priority != 0 && !ipAddress.equals("")){
				    	System.out.println(classNameLog + "Adding into hashmap => " + priority + " : " + ipAddress);
				    	hashMapFinal.put(priority, ipAddress);
				    	priority = 0;
				    	ipAddress = "";
				    }
				}
			}
		} else {
			//from JAVA
			System.out.println(classNameLog + "Value passed from JAVA");
			for(Map.Entry<String,String> entry : hashFromJava.entrySet()) {
				int key = Integer.parseInt(entry.getKey());
				String value = entry.getValue();
				System.out.println(classNameLog + "Key: Value =>" + key + ":" + value);
				hashMapFinal.put(key, value);
			}
			
		}
		
		return hashMapFinal;
	}
}
