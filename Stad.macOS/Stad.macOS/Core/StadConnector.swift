//
//  StadConnector.swift
//  Stad.macOS
//
//  Created by 박근희 on 2020/12/26.
//

import GRPC
import NIO

public class StadConnector {
    static var _isConnected: Bool = false
    public static func isConnected() -> Bool {
        return _isConnected
    }
    
    static func connect(host: String, port: Int) -> Bool {
        // Setup an `EventLoopGroup` for the connection to run on.
        //
        // See: https://github.com/apple/swift-nio#eventloops-and-eventloopgroups
        let group = MultiThreadedEventLoopGroup(numberOfThreads: 1)
        print("create group")
        
        // Make sure the group is shutdown when we're done with it.
        defer {
            try! group.syncShutdownGracefully()
        }
        
        // Configure the channel, we're not using TLS so the connection is `insecure`.
        let channel = ClientConnection.insecure(group: group)
            .connect(host: "localhost", port: 46755)
        print("connect")
        
        // Close the connection when we're done with it.
        defer {
            print("close")
            try! channel.close().wait()
        }
        do {
            // Provide the connection to the generated client.
            print("try hello")
            let client = Stad_Client_StadServiceClient(channel: channel)
            let sayHello = client.sayHello(Stad_Client_HelloRequest())
            let helloReply = try sayHello.response.wait()
            print("Hello reply")
            print (helloReply)
        }
        catch {
            print("hello failed: \(error)")
        }

        return true
    }
}
