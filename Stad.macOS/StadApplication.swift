//
//  Application.swift
//  Stad.macOS
//
//  Created by 박근희 on 2020/12/26.
//

import Foundation

enum AppSceneType : Int {
    case Undefined = 1
    case Connect
    case SourceSelect
}

class StadApplication {
    static var _appSceneType = AppSceneType.Connect
    static func setAppSceneType(type: AppSceneType) {
        _appSceneType = type
    }
    static func getAppSceneType() -> AppSceneType {
        return _appSceneType
    }
}
