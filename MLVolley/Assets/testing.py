import importlib_metadata

for entry in importlib_metadata.entry_points().get('mlagents.trainer_type', []):
    print(entry)

print('asdf')