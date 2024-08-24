﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace GameFrameX.CodeGenerator.Utils
{
    public static class ClassDeclarationSyntaxExt
    {
        public const string NESTED_CLASS_DELIMITER = "+";
        public const string NAMESPACE_CLASS_DELIMITER = ".";

        public static string GetFullName(this ClassDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var items = new List<string>();
            var parent = source.Parent;
            while (parent.IsKind(SyntaxKind.ClassDeclaration))
            {
                var parentClass = parent as ClassDeclarationSyntax;
                Contract.Assert(null != parentClass);
                items.Add(parentClass.Identifier.Text);

                parent = parent.Parent;
            }
            if (parent is FileScopedNamespaceDeclarationSyntax)
            {
                var nameSpace = parent as FileScopedNamespaceDeclarationSyntax;
                Contract.Assert(null != nameSpace);
                var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
                items.Reverse();
                items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
                sb.Append(source.Identifier.Text);

                var result = sb.ToString();
                return result;
            }
            else
            {
                var nameSpace = parent as NamespaceDeclarationSyntax;
                Contract.Assert(null != nameSpace);
                var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
                items.Reverse();
                items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
                sb.Append(source.Identifier.Text);

                var result = sb.ToString();
                return result;
            }
        }

        public static string GetNameSpace(this ClassDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var items = new List<string>();
            var parent = source.Parent;
            while (parent.IsKind(SyntaxKind.ClassDeclaration))
            {
                var parentClass = parent as ClassDeclarationSyntax;
                Contract.Assert(null != parentClass);
                items.Add(parentClass.Identifier.Text);

                parent = parent.Parent;
            }

            if (parent is FileScopedNamespaceDeclarationSyntax)
            {
                var nameSpace = parent as FileScopedNamespaceDeclarationSyntax;
                Contract.Assert(null != nameSpace);
                var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
                items.Reverse();
                items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
                sb.Append(source.Identifier.Text);

                var result = sb.ToString();
                return result;
            }
            else
            {
                var nameSpace = parent as NamespaceDeclarationSyntax;
                Contract.Assert(null != nameSpace);
                var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
                items.Reverse();
                items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
                sb.Append(source.Identifier.Text);

                var result = sb.ToString();
                return result;
            }



        }


    }
}
