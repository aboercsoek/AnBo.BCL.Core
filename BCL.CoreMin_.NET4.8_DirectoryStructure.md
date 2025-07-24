# Verzeichnisstruktur Altes Projekt (.NET 4.8)

## Planung und Stand der Migration

CoreMin/
├── Collections/
│   ├── DebugViews/
│   │   └── PropertyBagDebugView.cs
│   ├── Generic/
│   │   ├── DebugViews/
│   │   │   ├── *CollectionDebugView.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   │   ├── *DictionaryDebugView.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   │   └── *HashSetDebugView.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   ├── *HashSet.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   ├── *OrderedDictionary.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   ├── *ReadOnlyCollection.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   ├── *ReadOnlyDictionary.cs (Obsolet: Is in .NET 8+ vorhanden)
│   │   ├── TrackedList.cs
│   │   ├── TrackedListEnumerator.cs
│   │   ├── Tree.cs
│   │   ├── TreeNode.cs
│   │   └── TreeWalker.cs
│   ├── Graphs/
│   │   ├── DirectedGraph.cs
│   │   ├── Edge.cs
│   │   └── IEdge.cs
│   ├── IPropertyBag.cs
│   └── PropertyBag.cs
├── Core/
│   ├── AppUtil/
│   │   ├── AppHelper.s
│   │   └── GlobalLock.s
│   ├── **BooleanBoxes.cs (Migration fertig + Unittests)
│   ├── **BoolExtensions.cs (Migration fertig + Unittests)
│   ├── **GarbageCollectorEx.cs (Migration fertig + Unittests)
│   ├── **NumberFormatter.cs (Migration fertig + Unittests => Verschoben nach DataTypes/Converters)
│   └── **ObjectEx.cs (Migration fertig + Unittests => Verschoben nach Reflection)
├── CUI/
│   ├── ActionBasedMenuItemCmd.cs
│   ├── AppMenuController.cs
│   ├── CommandLine.cs
│   ├── ConsoleAppMenuView.cs
│   ├── ConsoleHelper.cs
│   ├── IAppMenuView.cs
│   ├── MenuItemCommandBase.cs
│   └── Paragraph.cs
├── Data/
│   ├── Extensions/
│   │   └── DataExtensions.cs
│   └── Utils/
│       └── DbDataHelper.cs
├── DataTypes/
│   ├── Algorithms/
│   │   ├── **Crc32Helper.cs (Migration fertig + Unittests)
│   │   ├── NameGenerator.cs
│   │   ├── PrimeFinder.cs
│   │   └── SequenceNumberGenerator.cs
│   ├── Attributes/
│   │   └── DisplayNameAttribute.cs
│   ├── Calculation/
│   │   ├── MathUtil.cs
│   │   └── UnitConverter.cs
│   ├── Compare/
│   │   ├── FuncBasedComparer.cs
│   │   ├── FuncBasedEqualityComparer.cs
│   │   ├── ReferenceEqualityComparerT.cs
│   │   └── StringEqualityComparer.cs
│   ├── Converters/
│   │   ├── Base64Converter.cs
│   │   ├── ByteArrayConverter.cs
│   │   ├── **HexConverter.cs (Migration fertig + Unittests)
│   │   └── **HexStringFormatOptions.cs (Migration fertig + Unittests)
│   ├── Enums/
│   │   ├── **EnumExtensions.cs (Migration fertig + Unittests)
│   │   └── **EnumHelper.cs (Migration fertig + Unittests)
│   ├── DisposeMethodTrigger.cs
│   ├── DoubleUtil.cs
│   ├── FloatUtil.cs
│   ├── **IndexValuePair.cs (Migration fertig + Unittests)
│   ├── Pair.cs
│   ├── PairEx.cs
│   ├── Singleton.cs
│   ├── TimestampedValue.cs
│   └── *Tuple.cs (Obsolet: Is in .NET 8+ vorhanden)
├── Diagnostics/
│   ├── ActionScopeTracer.cs
│   ├── CallStack.cs
│   ├── Console2File.cs
│   ├── Console2String.cs
│   ├── ConsoleTracer.cs
│   ├── DebugTextWriter.cs
│   ├── DebugTimer.cs
│   ├── InternalMethodTracer.cs
│   ├── ITracer.cs
│   ├── MethodTracer.cs
│   ├── OutputDebugStringTracer.cs
│   └── ScopeTracer.cs
├── Drawing/
│   ├── old/
│   │   ├── _ColorUtil.cs
│   │   ├── CmykaColor.cs
│   │   ├── ColorConverter.cs
│   │   ├── ColorHSL24Bit.cs
│   │   ├── ColorRGB24Bit.cs
│   │   ├── DrawingUtil.cs
│   │   ├── HexColor.cs
│   │   ├── HlsaColor.cs
│   │   ├── HsvaColor.cs
│   │   └── RgbaColor.cs
│   ├── ColorCMYK.cs
│   ├── ColorEx.cs
│   ├── ColorException.cs
│   ├── ColorHSL.cs
│   ├── ColorHSV.cs
│   ├── ColorRGB.cs
│   ├── ColorUtil.cs
│   └── KnownColors.cs
├── Error/
│   ├── ArgumentExceptions/
│   │   ├── *ArgDirectoryPathException.cs (Obsolet: Wird durch ArgumentExceptions ersetzt)
│   │   ├── *ArgEmptyException.cs (Obsolet: Wird durch ArgumentExceptions ersetzt)
│   │   ├── *ArgException.cs (Obsolet: Wird durch ArgumentExceptions ersetzt)
│   │   ├── *ArgFilePathException.cs (Obsolet: Wird durch ArgumentExceptions ersetzt)
│   │   ├── *ArgNullException.cs (Obsolet: Wird durch ArgumentNullExceptions ersetzt)
│   │   ├── *ArgNullOrEmptyException.cs (Obsolet: Wird durch ArgumentExceptions ersetzt)
│   │   └── *ArgOutOfRangeException.cs (Obsolet: Wird durch ArgumentOutOfRangeExceptions ersetzt)
│   ├── BusinessExceptions/
│   │   └── BrokenBusinessRuleException.cs
│   ├── DataTypeExceptions/
│   │   ├── CollectionReadOnlyException.cs
│   │   └── DateException.cs
│   ├── InteropExceptions/
│   │   └── Win32ExecutionException.cs
│   ├── IOExceptions/
│   │   ├── *DirectoryPathTooLongException.cs (Obsolet)
│   │   └── *FilePathTooLongException.cs (Obsolet)
│   ├── RuntimeExceptions/
│   │   ├── AccessToDisposedObjectException.cs
│   │   ├── InvalidOperationRequestException.cs
│   │   ├── InvalidTypeCastException.cs
│   │   ├── ObjectInitializationFailedException.cs
│   │   ├── OperationExecutionFailedException.cs
│   │   └── ProcessRunnerException.cs
│   ├── SecurityExceptions/
│   │   ├── ActiveDirectoryException.cs
│   │   ├── InvalidCredentialException.cs
│   │   ├── PermissionException.cs
│   │   ├── PrivilegeNotHeldException.cs
│   │   └── QueryCredentialDialogException.cs
│   ├── Utils/
│   │   ├── ExceptionHelper.cs
│   │   ├── ExceptionText.cs
│   │   └── ExceptionXmlHelper.cs
│   ├── BaseException.cs
│   ├── BusinessException.cs
│   ├── CombinedException.cs
│   ├── InfrastructureException.cs
│   └── TechException.cs
├── Interop/
│   ├── AdvApi32.cs
│   ├── ComputerNameFormat.cs
│   ├── CredUi.cs
│   ├── CredUiInfo.cs
│   ├── Kernel32.cs
│   ├── LogonFlags.cs
│   ├── LUID.cs
│   ├── LUID_AND_ATTRIBUTES.cs
│   ├── Netapi32.cs
│   ├── ProcessMemoryInformation.cs
│   ├── Psapi.cs
│   ├── SafeCloseHandle.cs
│   ├── SafeCryptProvHandle.cs
│   ├── SafeGlobalAllocHandle.cs
│   ├── SafeServiceHandle.cs
│   ├── SafeTokenPrivileges.cs
│   ├── SafeUserToken.cs
│   ├── SecurityImpersonationLevel.cs
│   ├── SecurityStatus.cs
│   ├── ServiceDescription.cs
│   ├── SYSTEMTIME.cs
│   ├── TOKEN_PRIVILEGE.cs
│   ├── TokenInformationClass.cs
│   ├── Win32.cs
│   ├── Win32Helper.cs
│   ├── WindowsStatusCode.cs
│   └── WinSecurityContext.cs
├── IO/
│   ├── Extensions/
│   │   ├── DirectoryInfoExtensions.cs
│   │   ├── FileInfoExtensions.cs
│   │   ├── StreamExtensions.cs
│   │   └── StreamReaderExtensions.cs
│   ├── Utils/
│   │   ├── **FileHelper.cs (Migration fertig + Unittests => Umbenennung in FileSystemManager.cs)
│   │   ├── PathHelper.cs
│   │   ├── StreamHelper.cs
│   │   └── StreamReaderHelper.cs
│   ├── CsvWriter.cs
│   ├── DgmlGraphWriter.cs
│   ├── DgmlStyleTargetType.cs
│   ├── ExcelXmlWriter.cs
│   ├── MultiFileWatcher.cs
│   ├── SeekableReadOnlyStream.cs
│   ├── UnclosableStream.cs
│   └── VirtualStream.cs
├── Linq/
│   ├── **ArrayExtensions.cs  (Migration fertig + Unittests)
│   ├── CollectionHelper.cs
│   ├── DictionaryExtensions.cs
│   ├── EmptyArrayT.cs
│   ├── EmptyCollection.cs
│   ├── EmptyDictionary.cs
│   ├── ** EnumerableExtensions.cs (Migration fertig + Unittests)
│   ├── IteratorHelper.cs
│   ├── QueueExtensions.cs
│   └── StackExtensions.cs
├── Logging/
│   ├── LogContext.cs
│   ├── LogFormatHelper.cs
│   ├── LogMethod.cs
│   └── QuickLogger.cs
├── Messaging/
│   ├── BoolMessage.cs
│   ├── BoolMessageData.cs
│   ├── BoolMessageExtensions.cs
│   ├── BoolMessageItem.cs
│   ├── ErrorData.cs
│   ├── ErrorDataType.cs
│   ├── EventHelper.cs
│   ├── ExecuteHelper.BoolMessageData.cs
│   └── ExecuteHelper.cs
├── Properties/
│   └── ProjectAssemblyInfo.cs
├── Reflection/
│   ├── PeReader/
│   │   ├── CharacteristicsType.cs
│   │   ├── CoffHeader.cs
│   │   ├── Cor20Header.cs
│   │   ├── DataDir.cs
│   │   ├── FileRegion.cs
│   │   ├── ImageFileMachine.cs
│   │   ├── ImageSectionType.cs
│   │   ├── ImageSubsystem.cs
│   │   ├── MdStreamHeader.cs
│   │   ├── MetaDataHeaders.cs
│   │   ├── MetaDataTableHeader.cs
│   │   ├── ModuleException.cs
│   │   ├── ModuleHeaders.cs
│   │   ├── MsDosStub.cs
│   │   ├── OsHeaders.cs
│   │   ├── PeFileInfo.cs
│   │   ├── PeHeader.cs
│   │   ├── SectionHeader.cs
│   │   └── StorageSigAndHeader.cs
│   ├── AssemblyEx.cs
│   ├── AssemblyHelper.cs
│   ├── AttributeEx.cs
│   ├── DataContractTypeInfo.cs
│   ├── ExtensionMethodsReflectionHelper.cs
│   ├── GenericTypeReflectionHelper.cs
│   ├── MemberInfoEx.cs
│   ├── MethodInfoEx.cs
│   ├── ReflectionEx.cs
│   ├── ReflectionUtils.cs
│   ├── **TypeEx.cs (Migration fertig + Unittests)
│   ├── **TypeHelper.cs (Migration fertig + Unittests)
│   └── **TypeOf.cs (Migration fertig + Unittests)
├── RegularExpression/
│   ├── *RegexExpressionStrings.cs (Obsolet: Wurde in RegexPatterns überführt)
│   └── **RegexHelper.cs (Migration fertig + Unittests)
├── Resources/
│   └── SmartExpert.ico
├── Security/
│   ├── Authentication/
│   │   ├── ConsolePromptForCredential.cs
│   │   ├── CredentialEntry.cs
│   │   ├── CredentialManager.cs
│   │   ├── LogonProviderType.cs
│   │   ├── LogonType.cs
│   │   ├── QueryCredentialDialog.cs
│   │   ├── QueryCredentialError.cs
│   │   └── QueryCredentialOptions.cs
│   ├── Claims/
│   │   └── ClaimsHelper.cs
│   ├── Crypto/
│   │   ├── CryptographyHelper.cs
│   │   ├── HashAlgorithmType.cs
│   │   └── KeyedHashAlgorithmType.cs
│   ├── Identity/
│   │   ├── AccountType.cs
│   │   ├── IdentityInfo.cs
│   │   ├── IdentityResolver.cs
│   │   └── SidHelper.cs
│   ├── LSA/
│   │   └── LsaAccountManager.cs
│   ├── Privileges/
│   │   ├── Privilege.cs
│   │   ├── PrivilegeAction.cs
│   │   ├── PrivilegeCollection.cs
│   │   └── PrivilegeState.cs
│   ├── SecurityHelper.cs
│   └── UniqueSecurityId.cs
├── Serialization/
│   ├── BinaryFormatterWrapper.cs
│   ├── DataContractSerializerWrapper.cs
│   ├── DictionaryXmlSerializerHelper.cs
│   ├── ISerializationFormatter.cs
│   ├── NetDataContractSerializerWrapper.cs
│   ├── SerializationFormatterFactory.cs
│   ├── SerializationFormatters.cs
│   ├── SerializationHelper.cs
│   └── XmlSerializerWrapper.cs
├── SystemManagement/
│   ├── AppEventLog.cs
│   ├── EnvironmentHelper.cs
│   └── RegistryHelper.cs
├── SystemProcesses/
│   ├── LineReceivedEventArgs.cs
│   ├── ProcessHelper.cs
│   ├── ProcessMemoryInfo.cs
│   └── ProcessRunner.cs
├── SystemRuntime/
│   ├── Interop/
│   │   ├── AsmCacheLocationFlags.cs
│   │   ├── AsmNameObjCreateFlags.cs
│   │   ├── AsmNamePropertyFlags.cs
│   │   ├── AssemblyInfo.cs
│   │   ├── AssemblyInfoFlags.cs
│   │   ├── AssemblyNameDisplayFlags.cs
│   │   ├── IApplicationContext.cs
│   │   ├── IAssemblyCache.cs
│   │   ├── IAssemblyEnum.cs
│   │   └── IAssemblyName.cs
│   ├── AssemblyNameComparer.cs
│   ├── AssemblyNameWrapper.cs
│   ├── Gac.cs
│   └── GacManager.cs
├── SystemServices/
│   ├── ServiceControlManagerEx.cs
│   └── ServiceStartType.cs
├── Text/
│   ├── Converters/
│   │   ├── *StringConverter.cs (Obsolet: Wurde in StringExtensions integriert)
│   │   └── StringParser.cs
│   ├── Extensions/
│   │   ├── **StringBuilderEx.cs (Migration fertig + Unittests)
│   │   └── **StringEx.cs (Migration fertig + Unittests => Namensänderung: StringExtensions.cs)
│   └── Utils/
│       ├── StringBuilderHelper.cs
│       └── **StringHelper.cs (Migration fertig + Unittests)
├── Threading/
│   ├── ActiveTimeTrigger.cs
│   ├── AsyncHelper.cs
│   ├── AsyncHelper.Extensions.cs
│   ├── AutoReleaseReaderWriterLock.cs
│   └── SpinWaitLock.cs
├── Time/
│   └── Utils/
│       ├── **DateTimeHelper.cs (Migration fertig + Unittests => Verschoben nach DataTypes/DateAndTime)
│       └── **MonthType.cs (Migration fertig + Unittests => Verschoben nach DataTypes/DateAndTime)
├── Validation/
│   ├── **ArgChecker.cs (Migration fertig + Unittests)
│   ├── ResharperAnnotations.cs
│   └── Validator.cs
├── Web/
│   ├── WcfHelper.cs
│   └── WebHelper.cs
├── WinForms/
│   ├── ApplicationHelper.cs
│   ├── ClipboardUtil.cs
│   └── HtmlCLipboardData.cs
├── Xml/
│   ├── Linq/
│   │   ├── XAttributeExtentions.cs
│   │   ├── XDocumentExtentions.cs
│   │   ├── XElementEqualyComparer.cs
│   │   └── XElementExtentions.cs
│   ├── XPath/
│   │   └── XPathHelper.cs
│   ├── Xsd/
│   │   └── XsdHelper.cs
│   ├── XmlHelper.cs
│   ├── XmlNodeExtensions.cs
│   └── XmlNodeListExtensions.cs
├── Settings.StyleCop
├── SmartExpert.BCL.CoreMin.netfx40.csproj
├── SmartExpert.BCL.CoreMin.netfx40.csproj.DotSettings
├── SmartExpert.BCL.CoreMin.netfx40.ruleset
├── SmartExpert.BCL.CoreMin.netfx48.csproj
├── SmartExpert.BCL.CoreMin.netfx48.csproj.DotSettings
├── **StringResources.cs (Migration fertig)
└── **StringResources.resx (Migration fertig)
