﻿using System;
using System.Collections.Generic;

namespace D2L.CodeStyle.UnsafeStaticCounter {

	internal sealed class AnalyzedResults {
		public readonly int UnsafeStaticsCount;
		public readonly int UnsafeNonReadonlyStaticsCount;
		public readonly IEnumerable<AnalyzedType> UnsafeStaticsPerType;
		public readonly IEnumerable<AnalyzedProject> UnsafeStaticsPerProject;
		public readonly IEnumerable<AnalyzedStatic> RawResults;

		public AnalyzedResults( AnalyzedStatic[] rawResults ) {
			RawResults = rawResults;

			UnsafeStaticsCount = rawResults.Length;

			var unsafeStaticsPerProject = new Dictionary<string, AnalyzedProject>();
			var unsafeStaticsPerType = new Dictionary<string, AnalyzedType>();

			foreach( var result in rawResults ) {

				// increment per project
				var project = unsafeStaticsPerProject.GetOrAdd(
						result.ProjectName,
						() => new AnalyzedProject( result.ProjectName )
					);
				project.UnsafeStaticsCount++;

				switch( result.Cause ) {

					// increment per-type
					case AnalyzedStatic.CAUSE_MUTABLE_TYPE:
						var analyzedType = unsafeStaticsPerType.GetOrAdd(
							result.FieldOrPropType,
							() => new AnalyzedType( result.FieldOrPropType )
						);
						analyzedType.UnsafeStaticsCount++;
						break;

					// increment readonly count
					case AnalyzedStatic.CAUSE_MUTABLE_DECLARATION:
						UnsafeNonReadonlyStaticsCount++;
						break;

					default:
						throw new InvalidOperationException( $"unknown cause{result.Cause}" );

				}

			}

			UnsafeStaticsPerProject = unsafeStaticsPerProject.Values;
			UnsafeStaticsPerType = unsafeStaticsPerType.Values;
		}
	}

	internal sealed class AnalyzedProject {
		public readonly string Name;
		public int UnsafeStaticsCount;

		public AnalyzedProject( string projectName ) {
			Name = projectName;
		}
	}
	internal sealed class AnalyzedType {
		public readonly string Name;
		public int UnsafeStaticsCount;

		public AnalyzedType( string typeName ) {
			Name = typeName;
		}
	}
}
