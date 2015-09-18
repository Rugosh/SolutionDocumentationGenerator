﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Model {
    public class Message : DocumentableElement {

        public string Name;

        public string Text;

        public LinkedList<string> PlaceHolderDataTypes;

        public Message()
            : base() {
            this.Name = string.Empty;
            this.Text = string.Empty;
            this.PlaceHolderDataTypes = new LinkedList<string>();
        }

        public override string ToString() {
            var retString = new StringBuilder();
            //retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Message: {0}", Name));
            retString.AppendLine(string.Format("\tText: {0}", Text));

            foreach (var i in PlaceHolderDataTypes) {
                retString.AppendLine(string.Format("\tPlace Holder Data Type: {0}", i));
            }

            return retString.ToString();
        }
    }
}
