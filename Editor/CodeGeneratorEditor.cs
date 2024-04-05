#if UNITY_EDITOR

using UnityEditor;

using Utils;
using Utils.CodeGen.Core;
using Utils.CodeGen;

[InitializeOnLoad]
public class CodeGeneratorEditor : EditorWindow
{
	[MenuItem("KaNet/Generate Network Tpyes Code")]
	public static void GenerateSyncCode()
	{
		GenerateCdoe(new NetworkPrimitiveTypesCodeGenerator());
		GenerateCdoe(new NetworkEnumTypesCodeGenerator());
		//GenerateCdoe(new NetworkClassTypesCodeGenerator());
	}

	public static void GenerateCdoe(CodeGenerator codeGenerator)
	{
		// Try load code generate option
		string optionPath = EditorGlobal.GetPathOnResourcesDataPath(codeGenerator.OptionFilePath);

		if (JsonHandler.TryLoadFromFile<CodeGenOption>(optionPath, out var options))
		{
			codeGenerator.SetCodeGenOption(options);
		}
		else
		{
			codeGenerator.SetCodeGenOptionByDefault();
		}

		// Try load code template
		string codeTemplatePath = EditorGlobal.GetPathOnResourcesDataPath(codeGenerator.CodeTemplatePath);

		if (!FileHandler.TryLoadTextFromFile(codeTemplatePath, out var templateCode))
		{
			Ulog.LogError(UlogType.CodeGenerator, $"Template source code load error : {codeTemplatePath}");
			return;
		}

		codeGenerator.SetCodeTemplate(templateCode);

		// Try generate code
		if (!codeGenerator.TryGenerate(out var generatedCode))
		{
			Ulog.LogError(UlogType.CodeGenerator, $"Generatet code failed!");
			return;
		}

		// Try save code genearte option
		if (!codeGenerator.TryGetCodeGenOption(out var codeGenOption))
		{
			Ulog.LogError(UlogType.CodeGenerator, $"Code gen option get failed!");
			return;
		}

		if (!JsonHandler.TrySaveToFile(optionPath, codeGenOption))
		{
			Ulog.LogError(UlogType.CodeGenerator, $"save code gen option failed!");
			return;
		}

		// Try save generated code
		string codePath = EditorGlobal.GetPathOnApplicationDataPath(codeGenerator.CodeFilePath);
		if (!FileHandler.TrySaveToFile(codePath, generatedCode))
		{
			Ulog.LogError(UlogType.CodeGenerator, $"Save generated code failed!");
			return;
		}

		Ulog.Log($"[{nameof(NetworkClassTypesCodeGenerator)}] Synchronize code generated!");
	}
}

#endif