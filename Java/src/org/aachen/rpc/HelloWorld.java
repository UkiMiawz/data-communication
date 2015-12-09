package org.aachen.rpc;

public class HelloWorld {
	public String message(String name) {
        return "Message : Hello from remote place! " + name;
	}
}