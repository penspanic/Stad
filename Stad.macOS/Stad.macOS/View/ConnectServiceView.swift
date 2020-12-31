//
//  ConnectServiceView.swift
//  Stad.macOS
//
//  Created by 박근희 on 2020/12/26.
//

import SwiftUI

struct ConnectServiceView: View {
    var body: some View {
        VStack {
            Text("ConnectServiceView")
            Button(action: connect) {
                Text("Connect!")
            }
        }
    }
    func connect() {
        DispatchQueue.global(qos: .background).async {
            StadConnector.connect(host: "127.0.0.1", port: Constants.ServicePort)
        }
    }
}

struct ConnectServiceView_Previews: PreviewProvider {
    static var previews: some View {
        ConnectServiceView()
    }
}
