import { EnumsToArrayPipe } from './enums-to-array.pipe';

describe('EnumsToArrayPipe', () => {
  it('create an instance', () => {
    const pipe = new EnumsToArrayPipe();
    expect(pipe).toBeTruthy();
  });
});
