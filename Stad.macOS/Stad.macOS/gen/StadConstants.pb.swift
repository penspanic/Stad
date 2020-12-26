// DO NOT EDIT.
// swift-format-ignore-file
//
// Generated by the Swift generator plugin for the protocol buffer compiler.
// Source: StadConstants.proto
//
// For information on using the generated types, please see the documentation:
//   https://github.com/apple/swift-protobuf/

import Foundation
import SwiftProtobuf

// If the compiler emits an error on this type, it is because this file
// was generated by a version of the `protoc` Swift plug-in that is
// incompatible with the version of SwiftProtobuf to which you are linking.
// Please ensure that you are building against the same version of the API
// that was used to generate this file.
fileprivate struct _GeneratedWithProtocGenSwiftVersion: SwiftProtobuf.ProtobufAPIVersionCheck {
  struct _2: SwiftProtobuf.ProtobufAPIVersion_2 {}
  typealias Version = _2
}

public enum Stad_Client_SourceType: SwiftProtobuf.Enum {
  public typealias RawValue = Int
  case undefined // = 0
  case localFile // = 1
  case amazonS3 // = 2
  case UNRECOGNIZED(Int)

  public init() {
    self = .undefined
  }

  public init?(rawValue: Int) {
    switch rawValue {
    case 0: self = .undefined
    case 1: self = .localFile
    case 2: self = .amazonS3
    default: self = .UNRECOGNIZED(rawValue)
    }
  }

  public var rawValue: Int {
    switch self {
    case .undefined: return 0
    case .localFile: return 1
    case .amazonS3: return 2
    case .UNRECOGNIZED(let i): return i
    }
  }

}

#if swift(>=4.2)

extension Stad_Client_SourceType: CaseIterable {
  // The compiler won't synthesize support with the UNRECOGNIZED case.
  public static var allCases: [Stad_Client_SourceType] = [
    .undefined,
    .localFile,
    .amazonS3,
  ]
}

#endif  // swift(>=4.2)

// MARK: - Code below here is support for the SwiftProtobuf runtime.

extension Stad_Client_SourceType: SwiftProtobuf._ProtoNameProviding {
  public static let _protobuf_nameMap: SwiftProtobuf._NameMap = [
    0: .same(proto: "Undefined"),
    1: .same(proto: "LocalFile"),
    2: .same(proto: "AmazonS3"),
  ]
}