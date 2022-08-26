namespace MathGen.Genetic
{
	public interface IGeneticAlgorithm<Model>
	{
		// Before looping generations
		void LoadCandidate(Model model);

		// Reproduce and Mutate
		Model Mutate(Model model);
		Model Cross(Model modelA, Model modelB);

		// Testing
		void TestCandidate(Model model);

		// Genocide
		int Compare(Model modelA, Model modelB);

		// After looping generations
		Model[] GetChoosen();
	}
}
