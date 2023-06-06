using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Sisus.ComponentNames.EditorOnly
{
	internal static class UnityEventDrawerUtility
	{
		private const String kNoFunctionString = "No Function";

		private const String kInstancePath = "m_Target";
		private const String kInstanceTypePath = "m_TargetAssemblyTypeName";
		private const String kArgumentsPath = "m_Arguments";
		private const String kModePath = "m_Mode";
		private const String kMethodNamePath = "m_MethodName";
		private const String kCallsPath = "m_PersistentCalls.m_Calls";

		internal const String kFloatArgument = "m_FloatArgument";
		internal const String kIntArgument = "m_IntArgument";
		internal const String kObjectArgument = "m_ObjectArgument";
		internal const String kStringArgument = "m_StringArgument";
		internal const String kBoolArgument = "m_BoolArgument";
		internal const String kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";

		public static GenericMenu BuildFunctionSelectDropdownMenu(SerializedProperty unityEventProperty, Int32 index)
		{
			UnityEventBase dummyEvent = GetDummyEvent(unityEventProperty);
			SerializedProperty propertyRelative = unityEventProperty.FindPropertyRelative(kCallsPath);
			SerializedProperty listener = propertyRelative.GetArrayElementAtIndex(index);
			if(listener is null)
			{
				#if DEV_MODE
				Debug.LogWarning(propertyRelative.propertyPath + "[" + index + "] is null");
				#endif

				return null;
			}

			Object listenerTarget = listener.FindPropertyRelative(kInstancePath).objectReferenceValue;

			// Use more reflection if possible to call some of Unity's internal methods.
			// This way if Unity updates those methods, I will automatically also get all the updates.
			if(!BuildFunctionSelectDropdownMenuUsingInternalMethod(listener, dummyEvent, listenerTarget, out GenericMenu menu))
			{
				menu = BuildFunctionSelectDropdownMenu(listenerTarget, dummyEvent, listener);
			}

			OverrideNames(listenerTarget, ref menu);

			return menu;
		}

		private static void OverrideNames(Object listenerTarget, ref GenericMenu menu)
		{
			if(menu is null)
			{
				return;
			}

			Component[] components;
			GameObject gameObject = listenerTarget as GameObject;
			if(gameObject == null)
			{
				if(!(listenerTarget is Component listenerComponent) || listenerComponent == null)
				{
					return;
				}
				
				gameObject = listenerComponent.gameObject;
			}

			components = gameObject.GetComponents<Component>();

			PropertyInfo menuItemsProperty = typeof(GenericMenu).GetProperty("menuItems", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			IList menuItems = menuItemsProperty.GetValue(menu, null) as IList;
			Int32 count = menuItems.Count;
			if(count == 0)
			{
				return;
			}

			Type menuItemType = menuItems[0].GetType();
			FieldInfo contentField = menuItemType.GetField("content", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(contentField is null)
			{
				#if DEV_MODE
				Debug.LogWarning($"Field {menuItemType.FullName}.content not found.");
				#endif
				return;
			}

			Dictionary<String, String> namesAndOverridesCache = new Dictionary<String, String>();

			for(Int32 i = 0; i < count; i++)
			{
				System.Object menuItem = menuItems[i];
				GUIContent label = (GUIContent)contentField.GetValue(menuItem);
				String originalPath = label.text;
				String overridePath = GetOverridePath(components, originalPath, namesAndOverridesCache);
				label.text = overridePath;
			}
		}

		private static String GetOverridePath(Component[] components, String originalPath, Dictionary<String, String> cache)
		{
			Int32 originalNameEnd = originalPath.IndexOf('/');
			if(originalNameEnd <= 0)
			{
				return originalPath;
			}
				
			String originalName = originalPath.Substring(0, originalNameEnd);
			if(cache.TryGetValue(originalName, out String overrideName))
			{
				return overrideName + originalPath[originalNameEnd..];
			}

			Int32 nth = 0;
			String originalNameWithoutSuffix = originalName;
			if(originalName.EndsWith(")", StringComparison.Ordinal) && Int32.TryParse(originalName.Substring(originalName.Length - 2, 1), out Int32 parsedNth))
			{
				nth = parsedNth;
				originalNameWithoutSuffix = originalName.Substring(0, originalName.Length - 4); // For example " (1)";
			}

			Boolean isFullName = originalNameWithoutSuffix.IndexOf('.') != -1;
			Component target;
			if(isFullName)
			{
				target = components.Where(c => c != null && String.Equals(c.GetType().FullName, originalNameWithoutSuffix)).ElementAtOrDefault(nth);
			}
			else
			{
				target = components.Where(c => c != null && String.Equals(c.GetType().Name, originalNameWithoutSuffix)).ElementAtOrDefault(nth);
			}

			if(target == null)
			{
				return originalPath;
			}

			overrideName = target.GetName();
			if(overrideName.EndsWith(" ()"))
			{
				overrideName = overrideName.Substring(0, overrideName.Length - 3);
			}

			Int32 index = 0;
			String baseName = overrideName;
			while(cache.ContainsValue(overrideName))
			{
				index++;
				overrideName = baseName + " (" + index + ")";
			}
			cache.Add(originalName, overrideName);

			String overridePath = overrideName + originalPath.Substring(originalNameEnd);
			return overridePath;
		}

		private static Boolean BuildFunctionSelectDropdownMenuUsingInternalMethod(SerializedProperty listenerProperty, UnityEventBase dummyEvent, Object listenerTarget, out GenericMenu menu)
		{
			menu = default;

			if(listenerProperty == null)
			{
				#if DEV_MODE
				Debug.LogWarning("Listener SerializedProperty null.");
				#endif

				return false;
			}

			MethodInfo buildPopupListMethod = typeof(UnityEventDrawer).GetMethod("BuildPopupList", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			if(buildPopupListMethod == null)
			{
				#if DEV_MODE
				Debug.LogWarning("Method UnityEventDrawer.BuildPopupList not found.");
				#endif

				return false;
			}

			ParameterInfo[] parameters = buildPopupListMethod.GetParameters();
			if(parameters.Length != 3
				|| parameters[0].ParameterType != typeof(Object)
				|| parameters[1].ParameterType != typeof(UnityEventBase)
				|| parameters[2].ParameterType != typeof(SerializedProperty))
			{
				#if DEV_MODE
				Debug.LogWarning("Method UnityEventDrawer.BuildPopupList parameter list was not expected.");
				#endif

				return false;
			}

			System.Object[] args = new System.Object[] { listenerTarget, dummyEvent, listenerProperty };
			try
			{
				menu = buildPopupListMethod.Invoke(null, args) as GenericMenu;
				if(menu is null)
				{
					#if DEV_MODE
					Debug.LogWarning("UnityEventDrawer.BuildPopupList return value was not castable to GenericMenu.");
					#endif
					return false;
				}

				return true;
			}
			#if DEV_MODE
			catch(Exception ex)
			{
				Debug.LogWarning(ex.ToString());
			#else
			catch
			{
			#endif

				return false;
			}
		}

		private static UnityEventBase GetDummyEvent(SerializedProperty unityEventProperty)
		{
			//Use the SerializedProperty path to iterate through the fields of the inspected targetObject
			Object targetObject = unityEventProperty.serializedObject.targetObject;
			if(targetObject == null)
			{
				return new UnityEvent();
			}

			Type staticType = GetStaticTypeFromProperty(unityEventProperty);
			if(staticType.IsSubclassOf(typeof(UnityEventBase)))
			{
				return Activator.CreateInstance(staticType) as UnityEventBase;
			}
			return new UnityEvent();
		}

		private static Type GetStaticTypeFromProperty(SerializedProperty property)
		{
			Type classType = GetScriptTypeFromProperty(property);
			if(classType == null)
			{
				return null;
			}

			String fieldPath = property.propertyPath;
			
			PropertyInfo isReferencingAManagedReferenceFieldProperty = typeof(SerializedProperty).GetProperty("isReferencingAManagedReferenceField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Boolean isReferencingAManagedReference = (Boolean)isReferencingAManagedReferenceFieldProperty.GetValue(property, null);
			if(isReferencingAManagedReference)
			{
				// When the field we are trying to access is a dynamic instance, things are a bit more tricky
				// since we cannot "statically" (looking only at the parent class field types) know the actual
				// "classType" of the parent class.

				// The issue also is that at this point our only view on the object is the very limited SerializedProperty.

				// So we have to:
				// 1. try to get the FQN from for the current managed type from the serialized data,
				// 2. get the path *in the current managed instance* of the field we are pointing to,
				// 3. foward that to 'GetFieldInfoFromPropertyPath' as if it was a regular field,

				MethodInfo objectTypenameMethod = typeof(SerializedProperty).GetMethod("GetFullyQualifiedTypenameForCurrentTypeTreeInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				String objectTypename = objectTypenameMethod.Invoke(property, null) as String;
				GetTypeFromManagedReferenceFullTypeName(objectTypename, out classType);

				MethodInfo fieldPathMethod = typeof(SerializedProperty).GetMethod("GetPropertyPathInCurrentManagedTypeTreeInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				fieldPath = fieldPathMethod.Invoke(property, null) as String;
			}

			if(classType == null)
			{
				return null;
			}

			return GetStaticTypeFromPropertyPath(classType, fieldPath);
		}

		private static Boolean GetTypeFromManagedReferenceFullTypeName(String managedReferenceFullTypename, out Type managedReferenceInstanceType)
        {
            managedReferenceInstanceType = null;

            String[] parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                String assemblyPart = parts[0];
                String nsClassnamePart = parts[1];
                managedReferenceInstanceType = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return managedReferenceInstanceType != null;
        }

		private static Type GetStaticTypeFromPropertyPath(Type host, String path)
		{
			const String arrayData = @"\.Array\.data\[[0-9]+\]";
			// we are looking for array element only when the path ends with Array.data[x]
			Boolean lookingForArrayElement = Regex.IsMatch(path, arrayData + "$");
			// remove any Array.data[x] from the path because it is prevents cache searching.
			path = Regex.Replace(path, arrayData, ".___ArrayElement___");

			Type type = host;
			String[] parts = path.Split('.');
			for(Int32 i = 0; i < parts.Length; i++)
			{
				String member = parts[i];
				// GetField on class A will not find private fields in base classes to A,
				// so we have to iterate through the base classes and look there too.
				// Private fields are relevant because they can still be shown in the Inspector,
				// and that applies to private fields in base classes too.
				FieldInfo foundField = null;
				for(Type currentType = type; foundField == null && currentType != null; currentType = currentType.BaseType)
				{
					foundField = currentType.GetField(member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}

				if(foundField == null)
				{
					return null;
				}

				type = foundField.FieldType;
				// we want to get the element type if we are looking for Array.data[x]
				if(i < parts.Length - 1 && parts[i + 1] == "___ArrayElement___" && type.IsArrayOrList())
				{
					i++; // skip the "___ArrayElement___" part
					type = type.GetArrayOrListElementType();
				}
			}

			// we want to get the element type if we are looking for Array.data[x]
			if(lookingForArrayElement && type != null && type.IsArrayOrList())
			{
				type = type.GetArrayOrListElementType();
			}


			return type;
		}

		private static Boolean IsArrayOrList(this Type listType)
		{
			if(listType.IsArray)
			{
				return true;
			}

			if(listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return true;
			}

			return false;
		}

		private static Type GetArrayOrListElementType(this Type listType)
		{
			if(listType.IsArray)
			{
				return listType.GetElementType();
			}

			if(listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}

			return null;
		}

		private static Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			if(property.serializedObject.targetObject != null)
			{
				return property.serializedObject.targetObject.GetType();
			}

			// Fallback in case the targetObject has been destroyed but the property is still valid.
			SerializedProperty scriptProp = property.serializedObject.FindProperty("m_Script");

			if(scriptProp == null)
			{
				return null;
			}

			MonoScript script = scriptProp.objectReferenceValue as MonoScript;

			if(script == null)
			{
				return null;
			}

			return script.GetClass();
		}

		public static GenericMenu BuildFunctionSelectDropdownMenu(Object target, UnityEventBase dummyEvent, SerializedProperty listener)
		{
			//special case for components... we want all the game objects targets there!
			Object targetToUse = target;
			if(targetToUse is Component)
			{
				targetToUse = (target as Component).gameObject;
			}

			// find the current event target...
			SerializedProperty methodName = listener.FindPropertyRelative(kMethodNamePath);

			GenericMenu menu = new GenericMenu();

			menu.AddItem
			(
				new GUIContent(kNoFunctionString),
				String.IsNullOrEmpty(methodName.stringValue),
				ClearEventFunction,
				new UnityEventFunction(listener, null, null, PersistentListenerMode.EventDefined)
			);

			if(targetToUse == null)
			{
				return menu;
			}

			menu.AddSeparator("");

			// figure out the signature of this delegate...
			// The property at this stage points to the 'container' and has the field name
			Type delegateType = dummyEvent.GetType();

			// check out the signature of invoke as this is the callback!
			MethodInfo delegateMethod = delegateType.GetMethod("Invoke");
			Type[] delegateArgumentsTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();

			Dictionary<String, Int32> duplicateNames = new Dictionary<String, Int32>();
			Dictionary<String, Int32> duplicateFullNames = new Dictionary<String, Int32>();

			GeneratePopUpForType(menu, targetToUse, targetToUse.GetType().Name, listener, delegateArgumentsTypes);
			duplicateNames[targetToUse.GetType().Name] = 0;
			if(targetToUse is GameObject __gameObject)
			{
				Component[] comps = __gameObject.GetComponents<Component>();

				// Collect all the names and record how many times the same name is used.
				foreach(Component comp in comps)
				{
					if(comp == null)
					{
						continue;
					}

					String componentTypeName = comp.GetType().Name;

					Int32 duplicateIndex = 0;
					if(duplicateNames.TryGetValue(componentTypeName, out duplicateIndex))
					{
						duplicateIndex++;
					}

					duplicateNames[componentTypeName] = duplicateIndex;
				}

				foreach(Component comp in comps)
				{
					if(comp == null)
					{
						continue;
					}

					Type compType = comp.GetType();
					String targetName = compType.Name;
					Int32 duplicateIndex = 0;

					// Is this name used multiple times? If so then use the full name plus an index if there are also duplicates of this. (case 1309997)
					if(duplicateNames[compType.Name] > 0)
					{
						if(duplicateFullNames.TryGetValue(compType.FullName, out duplicateIndex))
						{
							targetName = $"{compType.FullName} ({duplicateIndex})";
						}
						else
						{
							targetName = compType.FullName;
						}
					}

					GeneratePopUpForType(menu, comp, targetName, listener, delegateArgumentsTypes);
					duplicateFullNames[compType.FullName] = duplicateIndex + 1;
				}
			}

			return menu;
		}

		private static void GeneratePopUpForType(GenericMenu menu, Object target, String targetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
		{
			List<ValidMethodMap> methods = new List<ValidMethodMap>();
			Boolean didAddDynamic = false;

			// skip 'void' event defined on the GUI as we have a void prebuilt type!
			if(delegateArgumentsTypes.Length != 0)
			{
				GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods, PersistentListenerMode.EventDefined);
				if(methods.Count > 0)
				{
					menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " + String.Join(", ", delegateArgumentsTypes.Select(e => GetTypeName(e)).ToArray())));
					AddMethodsToMenu(menu, listener, methods, targetName);
					didAddDynamic = true;
				}
			}

			methods.Clear();
			GetMethodsForTargetAndMode(target, new[] { typeof(Single) }, methods, PersistentListenerMode.Float);
			GetMethodsForTargetAndMode(target, new[] { typeof(Int32) }, methods, PersistentListenerMode.Int);
			GetMethodsForTargetAndMode(target, new[] { typeof(String) }, methods, PersistentListenerMode.String);
			GetMethodsForTargetAndMode(target, new[] { typeof(Boolean) }, methods, PersistentListenerMode.Bool);
			GetMethodsForTargetAndMode(target, new[] { typeof(Object) }, methods, PersistentListenerMode.Object);
			GetMethodsForTargetAndMode(target, new Type[] { }, methods, PersistentListenerMode.Void);
			if(methods.Count > 0)
			{
				if(didAddDynamic)
				{
					// AddSeperator doesn't seem to work for sub-menus, so we have to use this workaround instead of a proper separator for now.
					menu.AddItem(new GUIContent(targetName + "/ "), false, null);
				}

				if(delegateArgumentsTypes.Length != 0)
				{
					menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
				}

				AddMethodsToMenu(menu, listener, methods, targetName);
			}
		}

		private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, String targetName)
		{
			// Note: sorting by a bool in OrderBy doesn't seem to work for some reason, so using numbers explicitly.
			IEnumerable<ValidMethodMap> orderedMethods = methods.OrderBy(e => e.methodInfo.Name.StartsWith("set_") ? 0 : 1).ThenBy(e => e.methodInfo.Name);
			foreach(ValidMethodMap validMethod in orderedMethods)
			{
				AddFunctionsForScript(menu, listener, validMethod, targetName);
			}
		}

		private static void GetMethodsForTargetAndMode(Object target, Type[] delegateArgumentsTypes, List<ValidMethodMap> methods, PersistentListenerMode mode)
		{
			IEnumerable<ValidMethodMap> newMethods = CalculateMethodMap(target, delegateArgumentsTypes, mode == PersistentListenerMode.Object);
			foreach(ValidMethodMap m in newMethods)
			{
				ValidMethodMap method = m;
				method.mode = mode;
				methods.Add(method);
			}
		}

		private static IEnumerable<ValidMethodMap> CalculateMethodMap(Object target, Type[] t, Boolean allowSubclasses)
		{
			List<ValidMethodMap> validMethods = new List<ValidMethodMap>();
			if(target == null || t == null)
			{
				return validMethods;
			}

			// find the methods on the behaviour that match the signature
			Type componentType = target.GetType();
			List<MethodInfo> componentMethods = componentType.GetMethods().Where(x => !x.IsSpecialName).ToList();

			IEnumerable<PropertyInfo> wantedProperties = componentType.GetProperties().AsEnumerable();
			wantedProperties = wantedProperties.Where(x => x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null);
			componentMethods.AddRange(wantedProperties.Select(x => x.GetSetMethod()));

			foreach(MethodInfo componentMethod in componentMethods)
			{
				//Debug.Log ("Method: " + componentMethod);
				// if the argument length is not the same, no match
				ParameterInfo[] componentParamaters = componentMethod.GetParameters();
				if(componentParamaters.Length != t.Length)
				{
					continue;
				}

				// Don't show obsolete methods.
				if(componentMethod.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
				{
					continue;
				}

				if(componentMethod.ReturnType != typeof(void))
				{
					continue;
				}

				// if the argument types do not match, no match
				Boolean paramatersMatch = true;
				for(Int32 i = 0; i < t.Length; i++)
				{
					if(!componentParamaters[i].ParameterType.IsAssignableFrom(t[i]))
					{
						paramatersMatch = false;
					}

					if(allowSubclasses && t[i].IsAssignableFrom(componentParamaters[i].ParameterType))
					{
						paramatersMatch = true;
					}
				}

				// valid method
				if(paramatersMatch)
				{
					ValidMethodMap vmm = new ValidMethodMap
					{
						target = target,
						methodInfo = componentMethod
					};

					validMethods.Add(vmm);
				}
			}
			return validMethods;
		}

		private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method, String targetName)
		{
			PersistentListenerMode mode = method.mode;

			// find the current event target...
			Object listenerTarget = listener.FindPropertyRelative(kInstancePath).objectReferenceValue;
			String methodName = listener.FindPropertyRelative(kMethodNamePath).stringValue;
			PersistentListenerMode setMode = GetMode(listener.FindPropertyRelative(kModePath));
			SerializedProperty typeName = listener.FindPropertyRelative(kArgumentsPath).FindPropertyRelative(kObjectArgumentAssemblyTypeName);

			StringBuilder args = new StringBuilder();
			Int32 count = method.methodInfo.GetParameters().Length;
			for(Int32 index = 0; index < count; index++)
			{
				ParameterInfo methodArg = method.methodInfo.GetParameters()[index];
				args.Append($"{GetTypeName(methodArg.ParameterType)}");

				if(index < count - 1)
				{
					args.Append(", ");
				}
			}

			Boolean isCurrentlySet = listenerTarget == method.target
			                         && methodName == method.methodInfo.Name
			                         && mode == setMode;

			if(isCurrentlySet && mode == PersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
			{
				isCurrentlySet &= (method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == typeName.stringValue);
			}

			String path = GetFormattedMethodName(targetName, method.methodInfo.Name, args.ToString(), mode == PersistentListenerMode.EventDefined);
			menu.AddItem
			(
				new GUIContent(path),
				isCurrentlySet,
				SetEventFunction,
				new UnityEventFunction(listener, method.target, method.methodInfo, mode)
			);
		}

		private static PersistentListenerMode GetMode(SerializedProperty mode)
		{
			return (PersistentListenerMode)mode.enumValueIndex;
		}

		private static String GetTypeName(Type t)
		{
			if(t == typeof(Int32))
				return "int";
			if(t == typeof(Single))
				return "float";
			if(t == typeof(String))
				return "string";
			if(t == typeof(Boolean))
				return "bool";
			return t.Name;
		}

		private static String GetFormattedMethodName(String targetName, String methodName, String args, Boolean dynamic)
		{
			if(dynamic)
			{
				if(methodName.StartsWith("set_"))
					return $"{targetName}/{methodName.Substring(4)}";
				else
					return $"{targetName}/{methodName}";
			}
			else
			{
				if(methodName.StartsWith("set_"))
					return String.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
				else
					return $"{targetName}/{methodName} ({args})";
			}
		}

		private static void SetEventFunction(System.Object source)
		{
			((UnityEventFunction)source).Assign();
		}

		private static void ClearEventFunction(System.Object source)
		{
			((UnityEventFunction)source).Clear();
		}

		private struct UnityEventFunction
		{
			private readonly SerializedProperty listener;
			private readonly Object target;
			private readonly MethodInfo method;
			private readonly PersistentListenerMode mode;

			public UnityEventFunction(SerializedProperty listener, Object target, MethodInfo method, PersistentListenerMode mode)
			{
				this.listener = listener;
				this.target = target;
				this.method = method;
				this.mode = mode;
			}

			public void Assign()
			{
				// find the current event target...
				SerializedProperty listenerTarget = listener.FindPropertyRelative(kInstancePath);
				SerializedProperty listenerTargetType = listener.FindPropertyRelative(kInstanceTypePath);
				SerializedProperty methodName = listener.FindPropertyRelative(kMethodNamePath);
				SerializedProperty mode = listener.FindPropertyRelative(kModePath);
				SerializedProperty arguments = listener.FindPropertyRelative(kArgumentsPath);

				listenerTarget.objectReferenceValue = target;
				listenerTargetType.stringValue = method.DeclaringType.AssemblyQualifiedName;
				methodName.stringValue = method.Name;
				mode.enumValueIndex = (Int32)this.mode;

				if(this.mode == PersistentListenerMode.Object)
				{
					SerializedProperty fullArgumentType = arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName);
					ParameterInfo[] argParams = method.GetParameters();
					if(argParams.Length == 1 && typeof(Object).IsAssignableFrom(argParams[0].ParameterType))
						fullArgumentType.stringValue = argParams[0].ParameterType.AssemblyQualifiedName;
					else
						fullArgumentType.stringValue = typeof(Object).AssemblyQualifiedName;
				}

				ValidateObjectParamater(arguments, this.mode);

				listener.serializedObject.ApplyModifiedProperties();
			}

			private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
			{
				SerializedProperty fullArgumentType = arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName);
				SerializedProperty argument = arguments.FindPropertyRelative(kObjectArgument);
				Object argumentObj = argument.objectReferenceValue;

				if(mode != PersistentListenerMode.Object)
				{
					fullArgumentType.stringValue = typeof(Object).AssemblyQualifiedName;
					argument.objectReferenceValue = null;
					return;
				}

				if(argumentObj == null)
					return;

				Type t = Type.GetType(fullArgumentType.stringValue, false);
				if(!typeof(Object).IsAssignableFrom(t) || !t.IsInstanceOfType(argumentObj))
					argument.objectReferenceValue = null;
			}

			public void Clear()
			{
				// find the current event target...
				SerializedProperty methodName = listener.FindPropertyRelative(kMethodNamePath);
				methodName.stringValue = null;

				SerializedProperty mode = listener.FindPropertyRelative(kModePath);
				mode.enumValueIndex = (Int32)PersistentListenerMode.Void;

				listener.serializedObject.ApplyModifiedProperties();
			}
		}
	}

	struct ValidMethodMap
	{
		public Object target;
		public MethodInfo methodInfo;
		public PersistentListenerMode mode;
	}
}