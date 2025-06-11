export class RecipeModel {
  constructor(
    public id: number,
    public name: string,
    public ingredients: any[],
    public cookTimeMinutes: number,
    public cuisine: string,
    public image: string = ''
  ) {}
}
