using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace GameFrameX.CodeGenerator.Agent
{
    public class AgentFilter : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> AgentList { get; private set; } = new List<ClassDeclarationSyntax>();
        private readonly HashSet<string> _agentNameList = new HashSet<string>();
        private readonly List<ClassDeclarationSyntax> _partialList = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is ClassDeclarationSyntax syntax)) return;
            if (IsCompAgent(syntax))
            {
                AgentList.Add(syntax);
                _agentNameList.Add(syntax.Identifier.Text);
            }
            else
            {
                if (syntax.Modifiers.ToList().FindIndex(s => s.Text == "partial") >= 0)
                {
                    _partialList.Add(syntax);
                }
            }
        }

        public void ClearPartialList()
        {
            foreach (var p in _partialList)
            {
                if (_agentNameList.Contains(p.Identifier.Text))
                {
                    AgentList.Add(p);
                }
            }
            _partialList.Clear();
        }

        private bool IsCompAgent(ClassDeclarationSyntax source)
        {
            if (source.BaseList == null)
                return false;
            IEnumerable<BaseTypeSyntax> baseTypes = source.BaseList.Types.Select(baseType => baseType);
            var res = baseTypes.Any((baseType) =>
            {
                string baseName = baseType.ToString();
                return baseName.Contains("StateComponentAgent")
                      || baseName.Contains("FuncComponentAgent")
                      || baseName.Contains("QueryComponentAgent")
                      || baseName.Contains("BaseComponentAgent");
            });
            return res;
        }
    }
}
