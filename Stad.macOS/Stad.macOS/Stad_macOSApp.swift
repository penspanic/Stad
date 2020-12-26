//
//  Stad_macOSApp.swift
//  Stad.macOS
//
//  Created by 박근희 on 2020/12/26.
//

import SwiftUI

@main
struct Stad_macOSApp: App {
        var body: some Scene {
            WindowGroup {
                computedView().frame(minWidth: 500, minHeight: 400)
            }
        }
    func computedView() -> some View {
        if StadApplication.getAppSceneType() == AppSceneType.Connect {
            return AnyView(ConnectServiceView())
        }
        else {
            return AnyView(ContentView())
        }
    }
}
