// swift-tools-version: 6.2
// The swift-tools-version declares the minimum version of Swift required to build this package.

import PackageDescription

let package = Package(
    name: "ClientCore",
    products: [
        // ClientCore
        .library(
            name: "ClientCore",
            targets: ["ClientCore"]
        ),
        .library(
            name: "ToolBox",
            targets: ["ToolBox"]
        ),
    ],
    targets: [
        // Client Core
        .target(
            name: "ClientCore",
            dependencies: ["ToolBox"]
        ),
        .testTarget(
            name: "ClientCoreTests",
            dependencies: ["ClientCore", "ToolBox"]
        ),
        
        // ToolBox
        .target(name: "ToolBox")
    ]
)
