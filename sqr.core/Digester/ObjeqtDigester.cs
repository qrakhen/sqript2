﻿using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class ObjeqtDigester : Digester<Stack<Token>, Objeqt>
    {
        public override Objeqt digest(Stack<Token> input)
        {
            log.spam("in " + GetType().Name);

            input.process((current, index) => { 
                if (!current().isType(Token.Type.Identifier))
                    throw new SqrError("identifier expected");
                

            });

            return (Objeqt)Value.Null;
        }
    }
}
