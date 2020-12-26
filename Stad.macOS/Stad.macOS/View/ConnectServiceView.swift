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
        StadConnector.connect(host: "localhost", port: Constants.ServicePort)
    }
}

struct ConnectServiceView_Previews: PreviewProvider {
    static var previews: some View {
        ConnectServiceView()
    }
}
