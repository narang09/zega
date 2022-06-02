import { BackendEnumToArrayPipe } from './backend-enum-to-array.pipe';

describe('BackendEnumToArrayPipe', () => {
  it('create an instance', () => {
    const pipe = new BackendEnumToArrayPipe();
    expect(pipe).toBeTruthy();
  });
});
