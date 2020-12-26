# Which Swift to use.
SWIFT:=swift
# Where products will be built; this is the SPM default.
SWIFT_BUILD_PATH:=./.build
SWIFT_BUILD_CONFIGURATION=debug
SWIFT_FLAGS=--build-path=${SWIFT_BUILD_PATH} --configuration=${SWIFT_BUILD_CONFIGURATION} --enable-test-discovery
# Force release configuration (for plugins)
SWIFT_FLAGS_RELEASE=$(patsubst --configuration=%,--configuration=release,$(SWIFT_FLAGS))

# protoc plugins.
PROTOC_GEN_SWIFT=protoc-gen-swift
PROTOC_GEN_GRPC_SWIFT=protoc-gen-grpc-swift

SWIFT_BUILD:=${SWIFT} build ${SWIFT_FLAGS}
SWIFT_BUILD_RELEASE:=${SWIFT} build ${SWIFT_FLAGS_RELEASE}
SWIFT_TEST:=${SWIFT} test ${SWIFT_FLAGS}
SWIFT_PACKAGE:=${SWIFT} package ${SWIFT_FLAGS}

# Name of generated xcodeproj
XCODEPROJ:=Stad.macOS.xcodeproj

### Package and plugin build targets ###########################################

# all:
# 	${SWIFT_BUILD}

# Package.resolved:
# 	${SWIFT_PACKAGE} resolve

# .PHONY:
# plugins: ${PROTOC_GEN_SWIFT} ${PROTOC_GEN_GRPC_SWIFT}
# 	cp $^ .

# ${PROTOC_GEN_SWIFT}: Package.resolved
# 	${SWIFT_BUILD_RELEASE} --product protoc-gen-swift

# ${PROTOC_GEN_GRPC_SWIFT}: Sources/protoc-gen-grpc-swift/*.swift
# 	${SWIFT_BUILD_RELEASE} --product protoc-gen-grpc-swift

# interop-test-runner:
# 	${SWIFT_BUILD} --product GRPCInteroperabilityTests

# interop-backoff-test-runner:
# 	${SWIFT_BUILD} --product GRPCConnectionBackoffInteropTest

### Xcodeproj

.PHONY:
project: ${XCODEPROJ}

${XCODEPROJ}:
	${SWIFT_PACKAGE} generate-xcodeproj --output $@
	@-ruby scripts/fix-project-settings.rb GRPC.xcodeproj || \
		echo "Consider running 'sudo gem install xcodeproj' to automatically set correct indentation settings for the generated project."

### Protobuf Generation ########################################################

%.pb.swift: %.proto ${PROTOC_GEN_SWIFT}
	protoc $< \
		--proto_path=$(dir $<) \
		--plugin=${PROTOC_GEN_SWIFT} \
		--swift_opt=Visibility=Public \
		--swift_out=Stad.macOS/gen/

%.grpc.swift: %.proto ${PROTOC_GEN_GRPC_SWIFT}
	protoc $< \
		--proto_path=$(dir $<) \
		--plugin=${PROTOC_GEN_GRPC_SWIFT} \
		--grpc-swift_opt=Visibility=Public \
		--grpc-swift_out=Stad.macOS/gen/


STAD_SERVICE_PROTO=../protos/StadService.proto
STAD_SERVICE_PB=$(STAD_SERVICE_PROTO:.proto=.pb.swift)
STAD_SERVICE_GRPC=$(STAD_SERVICE_PROTO:.proto=.grpc.swift)

STAD_CONSTANTS_PROTO=../protos/StadConstants.proto
STAD_CONSTANTS_PB=$(STAD_CONSTANTS_PROTO:.proto=.pb.swift)

# Generates protobufs and gRPC client
.PHONY:
generate-stad: ${STAD_SERVICE_PB} ${STAD_SERVICE_GRPC} ${STAD_CONSTANTS_PB}


### Testing ####################################################################

### Misc. ######################################################################

.PHONY:
clean:
	-rm -rf Packages
	-rm -rf ${SWIFT_BUILD_PATH}
	-rm -rf ${XCODEPROJ}
	-rm -f Package.resolved